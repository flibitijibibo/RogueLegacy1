using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DS2DEngine
{
    public class TxtToBinConverter
    {
        public static void Convert(string inputPath)
        {
            string path = inputPath.Substring(0, inputPath.LastIndexOf(Path.DirectorySeparatorChar));
            string fileName = inputPath.Substring(inputPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            fileName = fileName.Replace(".txt", ".bin");

            string outputPath = path + Path.DirectorySeparatorChar + fileName;
            Console.WriteLine(outputPath);

            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    using (StreamReader sr = new StreamReader(inputPath))
                    {
                        while (!sr.EndOfStream)
                            writer.Write(sr.ReadLine());
                    }
                }
            }
        }
    }
}
