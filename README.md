SplitterSprite4
===============

# Overview
目指しているのは、みんなで部品を出し合ってゲームを作り上げるゲームエンジンです。

どんなことを実現したいかは以下のスライドを見てね

https://drive.google.com/drive/folders/1RTZnWJKwT2dWChWv6zCFFbov6lxTen0h

# Status
現在、がんばって作成中です。
使ってみたい人はもう少し待っててね。

# フォルダ構成
```
SplitterSprite4/ ... ルートフォルダ
└SplitterSprite4/ ... C#ソリューションフォルダ
  ├ Common/ ... OS間共通処理用プロジェクト
  ├ ForCocoa/ ... MacOS向け表示用プロジェクト
  │                 - フレームワーク: Cocoa
  │                 - 開発状態: 中断
  │                 - 用途: MacOS向けリリース用
  ├ ForConsole/ ... コンソール表示用プロジェクト
  │                   - フレームワーク: なし(CUIのみ)
  │                   - 開発状態: 実施中
  │                   - 用途: テスト用
  ├ ForWPF/ ... Windows向け表示用プロジェクト
  │               - フレームワーク: Windows Presentatoin Foundation(WPF)
  │               - 開発状態: 実施中
  │               - 用途: Windows向けリリース用
  └ SpIDEr/ ... ゲーム制作ツールSpIDEr用プロジェクト
```
