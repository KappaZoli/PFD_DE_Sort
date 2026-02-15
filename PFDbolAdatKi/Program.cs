using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using System.Linq;
using System.Collections;
using static Program;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

class Program
{
    public class meret
    {
        public double kisebbMeret { get; set; }
        public double nagyobbMeret { get; set; }
        public int darab { get; set; }

    }


    public class PdfPageDimensions
    {
        public string FilePath { get; set; }
        public int PageNumber { get; set; }
        public double SmallerSide { get; set; }
        public double LargerSide { get; set; }

        public override string ToString()
        {
            return $"{FilePath} - Page {PageNumber}: Widht = {SmallerSide:F2} mm, Lenght = {LargerSide:F2} mm";

        }
    }

    static void Main()
    {
        Console.WriteLine("Copy in the folder name: ");
        string startNev = Console.ReadLine();


        List<PdfPageDimensions> pdfDimensions = new List<PdfPageDimensions>();

        
        if (Directory.Exists(startNev))
        {
            ListPdfFiles(startNev, pdfDimensions);
            foreach (var item in pdfDimensions)
            {
                Console.WriteLine(item);
            }
            List<meret> listMeret = sizeToTxt(pdfDimensions);
            pdfLocation(pdfDimensions);
        }
        else
        {
            Console.WriteLine("This path doesn't exist");
            
        }
       


        Console.WriteLine("\n\n      ::::::::::       ::::    :::       :::::::::\r\n     :+:              :+:+:   :+:       :+:    :+:\r\n    +:+              :+:+:+  +:+       +:+    +:+ \r\n   +#++:++#         +#+ +:+ +#+       +#+    +:+  \r\n  +#+              +#+  +#+#+#       +#+    +#+   \r\n #+#              #+#   #+#+#       #+#    #+#    \r\n##########       ###    ####       #########      ");
        Console.ReadKey();
    }


    private static void pdfLocation(List<PdfPageDimensions> pdfDimensions)
    {
        StreamWriter fileKi = new StreamWriter(".fileNamePlace.txt");
        fileKi.WriteLine("File name:");
        foreach (var item in pdfDimensions)
        {
            string[] darabolt = item.FilePath.Split('\\');
            if (item.PageNumber == 1) fileKi.WriteLine($"{darabolt.Last()}");
        }
        fileKi.WriteLine("\nFile place:");
        foreach (var item in pdfDimensions)
        {
            fileKi.WriteLine($"{item.FilePath}");
        }
        fileKi.Close();
    }

    private static List<meret> sizeToTxt(List<PdfPageDimensions> pdfDimensions)
    {
        List<double> meretTemplate = new List<double>();
        double osszmeret = 0;

        foreach (var item in pdfDimensions)
        {
            double xSzorY = item.SmallerSide * item.LargerSide;
            if (!meretTemplate.Contains(item.SmallerSide * item.LargerSide))
            {
                meretTemplate.Add(xSzorY);
            }
            osszmeret += xSzorY;
        }
        meretTemplate.Sort();

        List<meret> meretekRendezve = new List<meret>();

        foreach (var itemTemp in meretTemplate)
        {

            foreach (var itemPdf in pdfDimensions)
            {
                if (itemTemp == itemPdf.SmallerSide * itemPdf.LargerSide)
                {

                    meretekRendezve.Add(new meret
                    {
                        kisebbMeret = itemPdf.SmallerSide,
                        nagyobbMeret = itemPdf.LargerSide,
                        darab = 0,
                    });
                    break;
                }
            }
        }
        List<meret> meretKivalogatott = new List<meret>();

        for (int i = 0; i < meretekRendezve.Count; i++)

        {
            if (!meretKivalogatott.Contains(meretekRendezve[i]))
            {
                meretKivalogatott.Add(meretekRendezve[i]);
            }

        }
        int darab = 0;
        foreach (var value in meretKivalogatott)
        {



            foreach (var item in pdfDimensions)
            {
                if (item.SmallerSide == value.kisebbMeret && item.LargerSide == value.nagyobbMeret) darab++;

            }

            value.darab = darab;

            darab = 0;
        }
        string[] beFile = File.ReadAllLines(".Price.txt");
        int[] arak = new int[6];
        for (int i = 1; i < beFile.Length; i++)
        {
            string[] darabolt = beFile[i].Split(' ');
            arak[i - 1] = Convert.ToInt32(darabolt[1]);
        }
        StreamWriter fileKi = new StreamWriter(".StandardPrintingSizes.txt");
        fileKi.WriteLine("Billing information:");
        fileKi.WriteLine("Standard Sizes");
        List<string> egyetMeret = new List<string>();
        foreach (var item in meretKivalogatott)
        {
            //A4
            if (item.kisebbMeret == 210 && item.nagyobbMeret == 297) fileKi.WriteLine($"A4 (210*297): {item.darab}pcs {item.darab * arak[0]} Euro");
            //A3
            else if (item.kisebbMeret == 297 && item.nagyobbMeret == 420) fileKi.WriteLine($"A3 (297*420): {item.darab}pcs {item.darab * arak[1]} Euro");
            //A2
            else if (item.kisebbMeret == 420 && item.nagyobbMeret == 594) fileKi.WriteLine($"A2 (420*594): {item.darab}pcs {item.darab * arak[2]} Euro");
            //A1
            else if (item.kisebbMeret == 594 && item.nagyobbMeret == 841) fileKi.WriteLine($"A1 (594*841): {item.darab}pcs {item.darab * arak[3]} Euro");
            //A0
            else if (item.kisebbMeret == 841 && item.nagyobbMeret == 1189) fileKi.WriteLine($"A0 (841*1189): {item.darab}pcs {item.darab * arak[4]} Euro");
            //egyeb
            else egyetMeret.Add($"{item.kisebbMeret}*{item.nagyobbMeret}: {item.darab}pcs");
        }
        fileKi.WriteLine();
        fileKi.WriteLine("Irregular sizes");
        foreach (var item in egyetMeret)
        {
            fileKi.WriteLine(item);
        }   
        fileKi.WriteLine($"\nSum: {osszmeret / 1000000}m² {Math.Round(osszmeret * arak[5] / 1000000)} Euro");
        fileKi.Close();
        Console.WriteLine("Copy the location where to sort ");
        string path1 = Console.ReadLine().Trim();
        string str1 = System.IO.Path.Combine(path1, ".297");
        Directory.CreateDirectory(str1);
        string str2 = System.IO.Path.Combine(path1, ".420");
        Directory.CreateDirectory(str2);
        string str3 = System.IO.Path.Combine(path1, ".594");
        Directory.CreateDirectory(str3);
        string str4 = System.IO.Path.Combine(path1, ".841");
        Directory.CreateDirectory(str4);
        string str5 = System.IO.Path.Combine(path1, ".1189");
        Directory.CreateDirectory(str5);
        string str6 = System.IO.Path.Combine(path1, ".WTF");
        Directory.CreateDirectory(str6);
        int num5 = 0;
        foreach (PdfPageDimensions pdfDimension in pdfDimensions)
        {
            ++num5;
            Console.Write($"\r[{num5}/{pdfDimensions.Count}]");
            string fileName = System.IO.Path.GetFileName(pdfDimension.FilePath);
            string celMappa;

            if (pdfDimension.SmallerSide <= 297.0)
            {
                celMappa = str1; 
            }
            else if (pdfDimension.SmallerSide <= 420.0)
            {
                celMappa = str2; 
            }
            else if (pdfDimension.SmallerSide <= 594.0)
            {
                celMappa = str3; 
            }
            else if (pdfDimension.SmallerSide <= 841.0)
            {
                celMappa = str4; 
            }
            else if (pdfDimension.SmallerSide <= 1189.0)
            {
                celMappa = str5; 
            }
            else
            {
                celMappa = str6; 
            }

            string destFileName = System.IO.Path.Combine(celMappa, fileName);
            File.Copy(pdfDimension.FilePath, destFileName, true);
        }
        return meretekRendezve;
    }

    static void ListPdfFiles(string folderPath, List<PdfPageDimensions> pdfDimensions)
    {
        try
        {

            string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf");

            foreach (string filePath in pdfFiles)
            {
                DisplayPdfDimensions(filePath, pdfDimensions);
            }


            string[] subDirectories = Directory.GetDirectories(folderPath);

            foreach (string subDirectory in subDirectories)
            {
                ListPdfFiles(subDirectory, pdfDimensions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }
    }

    static void DisplayPdfDimensions(string filePath, List<PdfPageDimensions> pdfDimensions)
    {
        try
        {
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                int numberOfPages = pdfDoc.GetNumberOfPages();
                for (int i = 1; i <= numberOfPages; i++)
                {
                    PdfPage page = pdfDoc.GetPage(i);
                    Rectangle pageSize = page.GetPageSize();
                    double widthInMm = pageSize.GetWidth() * 0.352778;
                    double heightInMm = pageSize.GetHeight() * 0.352778;
                    if (widthInMm > heightInMm)
                    {
                        pdfDimensions.Add(new PdfPageDimensions
                        {
                            FilePath = filePath,
                            PageNumber = i,
                            SmallerSide = Math.Round(heightInMm),
                            LargerSide = Math.Round(widthInMm)
                        });
                    }
                    else
                    {
                        pdfDimensions.Add(new PdfPageDimensions
                        {
                            FilePath = filePath,
                            PageNumber = i,
                            SmallerSide = Math.Round(widthInMm),
                            LargerSide = Math.Round(heightInMm)
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not load the file {filePath}, Error: {ex.Message}");
        }
    }
}
