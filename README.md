# Unity Export Crunched Action

Unityプロジェクトからクランチ圧縮されたテクスチャアセットをエクスポートするためのGitHub Actionsです。

## 機能
このアクションは以下の処理を行います：
- 指定された入力ディレクトリおよびそのサブディレクトリからPNGファイルをコピー
- Unityプロジェクトでクランチ圧縮を適用
- 指定された出力ディレクトリに圧縮済みファイルをエクスポート

## 使用方法

以下は、このアクションを使用する際の例です。

```yaml
name: Export Crunched Textures

on:
  push:
    branches:
      - main

jobs:
  export-textures:
    runs-on: windows-latest
    steps:
      - name: Run Unity Export Crunched Action
        uses: tp-jp/unity-export-crunched-action@v1
        with:
          input-path: 'path/to/input'
          output-path: 'path/to/output'
          unity-email: ${{ secrets.UNITY_EMAIL }}
          unity-password: ${{ secrets.UNITY_PASSWORD }}
          unity-license: ${{ secrets.UNITY_LICENSE }}
          unity-version: '2022.3.22f1'
          max-size: '2048'
          compression-quality: '50'
```

## 入力パラメータ

| パラメータ名            | 必須  | デフォルト値       | 説明                                                                 |
|-------------------------|-------|-------------------|----------------------------------------------------------------------|
| `input-path`           | 必須  | なし              | 入力ディレクトリのパス（サブディレクトリも対象）                      |
| `output-path`          | 必須  | なし              | 出力ディレクトリのパス                                              |
| `unity-email`          | 必須  | なし              | Unity認証用のメールアドレス                                         |
| `unity-password`       | 必須  | なし              | Unity認証用のパスワード                                             |
| `unity-license`        | 必須  | なし              | Unityライセンスキー                                                 |
| `unity-version`        | 任意  | `2022.3.22f1`     | 使用するUnityのバージョン                                           |
| `max-size`             | 任意  | `2048`            | テクスチャの最大サイズ（ピクセル単位）                              |
| `compression-quality`  | 任意  | `50`              | 圧縮品質（0-100）                                                   |

## 注意事項
- Unityの認証情報（メールアドレス、パスワード、ライセンスキー）はGitHub Secretsに保存してください。
- 入力ディレクトリのパス と 出力ディレクトリのパス は必ず `github.workspace` 配下に作成してください。  
  例: input-path: 'input' や output-path: 'output' のように指定します。
