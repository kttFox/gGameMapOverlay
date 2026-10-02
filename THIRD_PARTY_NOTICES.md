# Third-Party Notices

## GodiNavi
本ツールは [GodiNavi](https://github.com/GD-fandev/godinavi) の実装を参考にしています。
以下の処理は GodiNavi のソースコードを C# に移植したものです。

- `ImageAnalysis.TightenTextCrop` (元: `source/paddle_ocr_backend.py` の `tighten_text_crop`)
- `TextParsing.CanonicalizeForMatch` (元: `source/map_engine.py`)

```
MIT License

Copyright (c) 2026 GD-fandev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## PaddleOCR / RapidOCR (OCR モデル)
実行時にダウンロードする PP-OCRv4 認識モデル (`*_PP-OCRv4_rec_mobile.onnx`) は
[PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) のモデルを
[RapidOCR](https://github.com/RapidAI/RapidOCR) が ONNX 形式に変換・配布しているものです。
いずれも Apache License 2.0 です。前処理と CTC デコードは RapidOCR の実装に合わせています。

## ONNX Runtime
[Microsoft.ML.OnnxRuntime](https://github.com/microsoft/onnxruntime) — MIT License
