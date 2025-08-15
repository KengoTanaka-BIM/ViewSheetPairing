using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

// 名前衝突回避用エイリアス
using RView = Autodesk.Revit.DB.View;
using RViewSheet = Autodesk.Revit.DB.ViewSheet;

namespace ViewSheetPairingForm
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // ビューとシートを取得
            List<RView> views = new FilteredElementCollector(doc)
                .OfClass(typeof(RView))
                .Cast<RView>()
                .Where(v => !v.IsTemplate && v.ViewType != ViewType.Schedule) // スケジュールとテンプレート除外
                .ToList();

            List<RViewSheet> sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(RViewSheet))
                .Cast<RViewSheet>()
                .Where(s => !s.IsPlaceholder)
                .ToList();

            // ダイアログを開く
            using (PairingForm form = new PairingForm(views, sheets))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var pairs = form.SelectedPairs;

                    using (Transaction tx = new Transaction(doc, "ペアビュー一括配置"))
                    {
                        tx.Start();

                        foreach (var pair in pairs)
                        {
                            RView view = pair.view;
                            RViewSheet sheet = pair.sheet;

                            if (Viewport.CanAddViewToSheet(doc, sheet.Id, view.Id))
                            {
                                // シート中央に配置
                                BoundingBoxUV outline = sheet.Outline;
                                double centerX = (outline.Max.U + outline.Min.U) / 2;
                                double centerY = (outline.Max.V + outline.Min.V) / 2;
                                XYZ location = new XYZ(centerX, centerY, 0);

                                Viewport.Create(doc, sheet.Id, view.Id, location);
                            }
                            else
                            {
                                TaskDialog.Show("警告", $"ビュー '{view.Name}' はシート '{sheet.SheetNumber}' に追加できません。");
                            }
                        }

                        tx.Commit();
                    }
                }
            }

            return Result.Succeeded;
        }
    }

    public partial class PairingForm : System.Windows.Forms.Form

    {
        private DataGridView dgv;
        private Button okButton;
        private Button cancelButton;
        private List<RView> _views;
        private List<RViewSheet> _sheets;

        public List<(RView view, RViewSheet sheet)> SelectedPairs { get; private set; }

        public PairingForm(List<RView> views, List<RViewSheet> sheets)
        {
            _views = views;
            _sheets = sheets;
            InitializeComponent();
        }



        private void InitializeComponent()
        {
            this.Text = "ビューとシートのペアリング";
            this.Width = 800;
            this.Height = 500;

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Top;
            dgv.Height = 400;
            dgv.AllowUserToAddRows = true;
            dgv.AutoGenerateColumns = false;

            var viewColumn = new DataGridViewComboBoxColumn();
            viewColumn.HeaderText = "ビュー";
            viewColumn.DataSource = _views;
            viewColumn.DisplayMember = "Name"; // 表示用
            viewColumn.ValueMember = "Id";     // 識別用
            viewColumn.Width = 350;

            var sheetColumn = new DataGridViewComboBoxColumn();
            sheetColumn.HeaderText = "シート";
            sheetColumn.DataSource = _sheets;
            sheetColumn.DisplayMember = "SheetNumber"; // 表示用
            sheetColumn.ValueMember = "Id";            // 識別用
            sheetColumn.Width = 200;

            dgv.Columns.Add(viewColumn);
            dgv.Columns.Add(sheetColumn);

            // OKボタン
            okButton = new Button();
            okButton.Text = "OK";
            okButton.Dock = DockStyle.Bottom;
            okButton.Click += OkButton_Click;

            // キャンセルボタン
            cancelButton = new Button();
            cancelButton.Text = "キャンセル";
            cancelButton.Dock = DockStyle.Bottom;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(dgv);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SelectedPairs = new List<(RView, RViewSheet)>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    var viewId = (ElementId)row.Cells[0].Value;
                    var sheetId = (ElementId)row.Cells[1].Value;

                    RView view = _views.FirstOrDefault(v => v.Id == viewId);
                    RViewSheet sheet = _sheets.FirstOrDefault(s => s.Id == sheetId);

                    if (view != null && sheet != null)
                    {
                        SelectedPairs.Add((view, sheet));
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
