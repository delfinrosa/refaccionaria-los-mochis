using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using QRCoder; 
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace CapaDatos
{
    public class Recursos
    {
        // Cifrar contraseñas 
        public string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));
                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public List<(string ColumnName, int MaxLength)> Tamaño(string tableName)
        {
            List<(string ColumnName, int MaxLength)> columnLengths = new List<(string, int)>();

            string query = @"
            SELECT COLUMN_NAME, CHARACTER_MAXIMUM_LENGTH
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @TableName";

            using (SqlConnection connection = new SqlConnection(Conexion.cn))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tableName);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string columnName = reader.GetString(0);
                    int maxLength = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
                    columnLengths.Add((columnName, maxLength));
                }
            }

            return columnLengths;
        }

        public Bitmap GenerarCodigoQR(string texto)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(2);
            return qrCodeImage;
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }


        public Bitmap GenerarCodigoDeBarras(string texto)
        {
            const int ppi = 96; // Resolución de 96 píxeles por pulgada
            double anchoCm = 7.5; // Ancho predefinido en cm
            double altoCm = 6;  // Alto predefinido en cm

            int anchoPx = (int)((anchoCm / 2.54) * ppi);
            int altoPx = (int)((altoCm / 2.54) * ppi);

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE, 
                Options = new EncodingOptions
                {
                    Width = anchoPx,
                    Height = altoPx
                },
                Renderer = new BitmapRenderer()
            };
            return writer.Write(texto);
        }
    }
}
