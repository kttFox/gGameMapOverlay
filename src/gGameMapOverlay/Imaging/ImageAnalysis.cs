using System.Drawing;

namespace gGameMapOverlay.Imaging;

public static class ImageAnalysis
{
    /// <summary>排他フルスクリーン等でキャプチャが真っ黒になったケースを検出する。</summary>
    public static bool IsBlackFrame(BgrImage image)
    {
        int low = 255, high = 0;
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var gray = image.Gray(x, y);
                low = Math.Min(low, gray);
                high = Math.Max(high, gray);
            }
        }
        return high <= 4 || (high - low <= 2 && high <= 8);
    }

    /// <summary>ノイズに強い簡易フィンガープリント。見た目が変わっていない座標の再 OCR を省く。</summary>
    public static string Signature(BgrImage image)
    {
        var small = image.ResizeBilinear(64, 16);
        var buffer = new char[64 * 16];
        for (var y = 0; y < 16; y++)
        {
            for (var x = 0; x < 64; x++)
            {
                buffer[y * 64 + x] = (char)('a' + small.Gray(x, y) / 16);
            }
        }
        return new string(buffer);
    }

    /// <summary>
    /// 白っぽい文字画素の外接矩形まで切り詰め、UI の枠線や余白を除く。
    /// GodiNavi (MIT License, Copyright (c) 2026 GD-fandev) の tighten_text_crop を移植したもの。
    /// 加えて、切り出し範囲の左右の端に接する成分は除く。領域を少し大きめに囲んだとき、
    /// 隣の UI (マップ名欄の左にある「マップ」ラベルの縁など) の断片を文字として読まないようにするため。
    /// </summary>
    public static BgrImage TightenTextCrop(BgrImage image)
    {
        int width = image.Width, height = image.Height;
        var mask = new bool[width * height];
        var rowCounts = new int[height];
        var columnCounts = new int[width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var i = (y * width + x) * 3;
                var b = image.Pixels[i];
                var g = image.Pixels[i + 1];
                var r = image.Pixels[i + 2];
                var min = Math.Min(r, Math.Min(g, b));
                var max = Math.Max(r, Math.Max(g, b));
                if (min >= 115 && max - min <= 75)
                {
                    mask[y * width + x] = true;
                    rowCounts[y]++;
                    columnCounts[x]++;
                }
            }
        }

        // 横一杯・縦一杯に伸びる明るい線は枠とみなして除外する。
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (rowCounts[y] > width * 0.45 || columnCounts[x] > height * 0.55)
                {
                    mask[y * width + x] = false;
                }
            }
        }

        // 8近傍の連結成分のうち、小さすぎる点ノイズと大きすぎる塊を捨てる。
        var visited = new bool[width * height];
        var queue = new Queue<int>();
        var component = new List<int>();
        int left = width, top = height, right = -1, bottom = -1, kept = 0;
        for (var start = 0; start < mask.Length; start++)
        {
            if (!mask[start] || visited[start])
            {
                continue;
            }
            component.Clear();
            visited[start] = true;
            queue.Enqueue(start);
            int cLeft = width, cTop = height, cRight = -1, cBottom = -1;
            while (queue.Count > 0)
            {
                var index = queue.Dequeue();
                component.Add(index);
                int px = index % width, py = index / width;
                cLeft = Math.Min(cLeft, px);
                cRight = Math.Max(cRight, px);
                cTop = Math.Min(cTop, py);
                cBottom = Math.Max(cBottom, py);
                for (var dy = -1; dy <= 1; dy++)
                {
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        int nx = px + dx, ny = py + dy;
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        {
                            continue;
                        }
                        var neighbor = ny * width + nx;
                        if (mask[neighbor] && !visited[neighbor])
                        {
                            visited[neighbor] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            var componentWidth = cRight - cLeft + 1;
            var componentHeight = cBottom - cTop + 1;
            var touchesSide = cLeft == 0 || cRight == width - 1;
            if (component.Count >= 3 && componentWidth < width * 0.8 && componentHeight < height * 0.8 && !touchesSide)
            {
                kept += component.Count;
                left = Math.Min(left, cLeft);
                right = Math.Max(right, cRight);
                top = Math.Min(top, cTop);
                bottom = Math.Max(bottom, cBottom);
            }
        }

        if (kept < 8)
        {
            return image;
        }
        return image.Crop(Rectangle.FromLTRB(
            Math.Max(0, left - 6),
            Math.Max(0, top - 6),
            Math.Min(width, right + 7),
            Math.Min(height, bottom + 7)));
    }
}
