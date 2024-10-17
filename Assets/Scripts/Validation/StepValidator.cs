using System.Collections;
using System.Collections.Generic;
using Ganeral;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Validation
{
    public static class StepValidator
    {
        /// <summary>
        /// validates given tile based on the transparency of pixels on it's edge based on given direction
        /// </summary>
        /// <param name="direction">which edge to check</param>
        /// <param name="tile">tile to validate</param>
        /// <returns></returns>
        public static bool IsValid(Vector3Int direction, Tile tile) //calculate on separate thread
        {
            Sprite sprite = tile.sprite;

            if (sprite != null)
            {
                Texture2D texture = sprite.texture;
                Rect spriteRect = sprite.textureRect;
                int width = (int)spriteRect.width;
                int height = (int)spriteRect.height;

                int transparentPixelCount = 0;
                int totalPixelCount = 0;

                Vector3Int normalizedDirection = CoordinateManager.NormalizeIntVector(direction);

                if (normalizedDirection == Vector3Int.left)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color pixelColor = texture.GetPixel((int)spriteRect.x, (int)(spriteRect.y + y));
                        totalPixelCount++;
                        if (IsPixelTransparent(pixelColor))
                        {
                            transparentPixelCount++;
                        }
                    }
                }
                else if (normalizedDirection == Vector3Int.right)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color pixelColor = texture.GetPixel((int)(spriteRect.x + width - 1), (int)(spriteRect.y + y));
                        totalPixelCount++;
                        if (IsPixelTransparent(pixelColor))
                        {
                            transparentPixelCount++;
                        }
                    }
                }
                else if (normalizedDirection == Vector3Int.up)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixelColor = texture.GetPixel((int)(spriteRect.x + x), (int)(spriteRect.y + height - 1));
                        totalPixelCount++;
                        if (IsPixelTransparent(pixelColor))
                        {
                            transparentPixelCount++;
                        }
                    }
                }
                else if (normalizedDirection == Vector3Int.down)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixelColor = texture.GetPixel((int)(spriteRect.x + x), (int)spriteRect.y);
                        totalPixelCount++;
                        if (IsPixelTransparent(pixelColor))
                        {
                            transparentPixelCount++;
                        }
                    }
                }
                float transparentPercentage = (float)transparentPixelCount / totalPixelCount;
                return transparentPercentage > 0.6f;
            }
            return false;
        }

        private static bool IsPixelTransparent(Color pixel) => pixel.a == 0;
    }
}
