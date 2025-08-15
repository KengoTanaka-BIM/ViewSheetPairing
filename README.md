# ViewSheetPairingForm

Revit 2026 用のビューとシートのペアリング アドインです。  
左側にビュー、右側にシートを一覧表示し、同じ行で選択したビューとシートを **一括でシートに配置** できます。

- 複数のビューとシートのペアをまとめて配置可能
- シート中央に自動で配置
- 配置できない場合は警告ダイアログで通知
- GUI 上でペアを自由に追加・編集可能

---

## インストール方法

1. このリポジトリをクローンまたはダウンロード  
2. `ViewSheetPairingForm.dll` を Revit の Addins フォルダに配置  
3. 以下のような `.addin` ファイルを作成して読み込む：

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Name>ViewSheetPairingForm</Name>
    <Assembly>C:\test\ViewSheetPairingForm\ViewSheetPairingForm\bin\Debug\ViewSheetPairingForm.dll</Assembly>
    <AddInId>12345678-ABCD-1234-ABCD-1234567890AB</AddInId>
    <FullClassName>ViewSheetPairingForm.Command</FullClassName>
    <VendorId>IKST</VendorId>
    <VendorDescription>KengoTanaka</VendorDescription>
  </AddIn>
</RevitAddIns>
```

## 使用方法

1. Revit でアドインを起動
2. ダイアログで左側のビューと右側のシートを選択
3. OK ボタンを押すと、選択ペアを一括配置
4. 配置できないビューは警告が表示される

---

## 将来の構想（TODO）

- ペアリングの自動提案機能
- 配置位置のカスタマイズ
- 複数シートへの同時コピー
- GUI 表示改善（列幅自動調整など）
- ログ出力機能

---

## 作者

田中 健悟  

BIMエンジニア。Revit APIによるアドイン開発を専門としています。  

設備分野の実務経験と多国籍チームのマネジメントを経て、建設業界のDX推進を目指しています。  

副業でBIM効率化ツールを開発中。開発依頼やコラボ歓迎です。

Qiitaにて記事公開:  
[https://qiita.com/KengoTanaka-BIM/items/ee06deb9bc65dcfecc22](https://qiita.com/KengoTanaka-BIM/items/ee06deb9bc65dcfecc22)

---

## ライセンス & お問い合わせ

- ライセンス：MIT（※自由に使ってOK）  
- 質問・案件相談は Issues または GitHub Profile からどうぞ
