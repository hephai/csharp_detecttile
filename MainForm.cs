//----------------------------------------------------------------------------
//  Copyright (C) 2016 by hephaistion GmbH. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using Emgu.CV.Util;
using System.Linq;

namespace TileDetection
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        struct contourAreaS
        {
            public double area;
            public double hullArea;
            public int i;
        };

        public double hullAreasAvg(VectorOfVectorOfPoint contours)
        {
            contourAreaS contourArea;
            List<contourAreaS> contourAreasL = new List<contourAreaS>();
            for (int i = 0; i < contours.Size; i++)
            {
                //contourArea ausrechnen und nur die gréssten 4 einzeichnen, nach dem sortieren ist das i weg
                //das Average ausrechnen
                contourArea.area = CvInvoke.ContourArea(contours[i], false);
                contourArea.hullArea = 0;
                contourArea.i = i;
                contourAreasL.Add(contourArea);
                //                hullAreaAvg = hullAreaAvg + hullAreas[i];
                //hullAreaL.Add(hullAreas[i]);
            }
            //            hullAreaAvg = hullAreaAvg / contours.Size;
            contourAreasL.Sort((x, y) => y.area.CompareTo(x.area)); // die 4 grössten sind am Anfang

            return contourAreasL[3].area;
            // ca 14*14 = 196
        }

        public List<RotatedRect> rotRectsGtAreaEliminateSameCentered(VectorOfVectorOfPoint contours, double contourAreaMin, double contourAreaMax, double xdist, double ydist, List<int> iListFoundContours)
        {
            // die contours finden, die eine grösser als Min kleiner als Max sind, die übereinanderliegenden aussortieren
            // RotRects drumherummalen und als Liste zurückgeben und eine passende Liste mit den i's der contours

            // die contour suchen, die grésser ist als hullAreaMin mehr als 4 Punkte hat und convex ist 

            RotatedRect rrect = new RotatedRect();
            List<RotatedRect> rrects = new List<RotatedRect>();
            contourAreaS contourArea;
            List<contourAreaS> contourAreasL = new List<contourAreaS>();
            VectorOfVectorOfPoint hulls = new VectorOfVectorOfPoint(contours.Size);

            // für alle contours die Area berechnen und mit der contour nummer in hullAreasL speichern und nach area desc sortieren
            for (int i = 0; i < contours.Size; i++)
            {
                contourArea.area = CvInvoke.ContourArea(contours[i], false);
                CvInvoke.ConvexHull(contours[i], hulls[i]);
                contourArea.hullArea = CvInvoke.ContourArea(hulls[i], false);
                contourArea.i = i;
                contourAreasL.Add(contourArea);
            }
            contourAreasL.Sort((x, y) => y.area.CompareTo(x.area)); // die 4 grössten sind am Anfang

            for (int i = 0; i < contourAreasL.Count; i++)
            {
                bool rectUebereinander = false;
                if (contours[contourAreasL[i].i].Size >= 4)
                {
                    if (contourAreasL[i].hullArea > contourAreaMin && contourAreasL[i].hullArea < contourAreaMax) // fängt mit den grössten an wegen Sortierung
                    {
                        //èbereinanderliegende contours wegfiltern
                        //dazu mitte des rrect mit allen in rrects vergleichen, wenn die distance kleiner als 
                        // xdist < 1/4 xBild und ydist < 1/4 yBild, dann aussortieren (der Wert 1/4 xBild gilt nur bei Fiducials)
                        rrect = CvInvoke.MinAreaRect(contours[contourAreasL[i].i]);
                        if (rrects.Count > 0) // falls noch kein rrects da ist
                        {
                            for (int j = 0; j < rrects.Count; j++)
                            {
                                if ((Math.Abs(rrect.Center.X - rrects[j].Center.X) < xdist) && (Math.Abs(rrect.Center.Y - rrects[j].Center.Y) < ydist))
                                {
                                    rectUebereinander = true;
                                }
                            }
                            if (rectUebereinander == false)
                            {
                                iListFoundContours.Add(contourAreasL[i].i);

                                rrects.Add(rrect);
                                rectUebereinander = false;
                            }
                            else
                            {
                                // wenn rectUebereinander == true, dann nicht zu iListFoundContours dazupacken
                                rectUebereinander = false;
                            }
                        }
                        else
                        { //den ersten rrect einfach nehmen
                            iListFoundContours.Add(contourAreasL[i].i);
                            rrects.Add(rrect);
                        }
                    }
                }
            }

            return rrects;
        }

        public List<RotatedRect> quadRotRectClockwise(List<RotatedRect> rrects)
        {
            //funktion zum sortieren von 4 rotatedRect - input rrects, output rrectsClockwiseSort
            //wenn genau 4 rrect erkannt wurden, clockwise sortieren
            //links oben ist 0,0; x=column=width, y=row=height
            //Clockwise sortieren, erst nach y, die 2 mit kleinem x nach y absteigend, die 2 mit grossem x nach y aufsteigend
            List<RotatedRect> rrectsSortX = new List<RotatedRect>();
            List<RotatedRect> rrectsClockwiseSort = new List<RotatedRect>();
            if (rrects.Count == 4)
            {
                rrectsSortX = rrects.OrderBy(rr => rr.Center.X).ToList();
                // links Seite - klein x
                if (rrectsSortX[0].Center.Y >= rrectsSortX[1].Center.Y)
                {
                    rrectsClockwiseSort.Add(rrectsSortX[0]);
                    rrectsClockwiseSort.Add(rrectsSortX[1]);
                }
                else
                {
                    rrectsClockwiseSort.Add(rrectsSortX[1]);
                    rrectsClockwiseSort.Add(rrectsSortX[0]);
                }
                // rechte Seite -- gross x
                if (rrectsSortX[2].Center.Y <= rrectsSortX[3].Center.Y)
                {
                    rrectsClockwiseSort.Add(rrectsSortX[2]);
                    rrectsClockwiseSort.Add(rrectsSortX[3]);
                }
                else
                {
                    rrectsClockwiseSort.Add(rrectsSortX[3]);
                    rrectsClockwiseSort.Add(rrectsSortX[2]);
                }
            }
            return rrectsClockwiseSort;
        }

        public Mat detectFiducialPointsPerspectiveMat(Image<Bgr, Byte> in_image, IImage transform_output, MCvScalar lower, MCvScalar upper, int width, int height)
        {
            Mat transfMat = new Mat(); //return
            PointF[] dstTransfRect = new PointF[4] { new Point(0, height), new Point(0, 0), new Point(width, 0), new Point(width, height) };

            // Gelb erkennen und mit InRange filtern

            //UMat yellow_output = new UMat(in_image.Size, DepthType.Cv8U, 1);
            //geht mit UMat nicht, da Canny einen Bug hat http://code.opencv.org/issues/4120

            //Image<Hsv, Byte> hsv_image = new Image<Hsv, byte>(in_image.Size);
            //CvInvoke.CvtColor(in_image, hsv_image, ColorConversion.Bgr2Hsv);

            //Image<Bgr, Byte> blur_output = new Image<Bgr, byte>(in_image.Size); // das bringt nix
            //CvInvoke.GaussianBlur(in_image, blur_output, new Size(5, 5), 1.5);

            Image<Gray, Byte> yellow_output = new Image<Gray, Byte>(in_image.Size);

            /*            ScalarArray loweri = new ScalarArray(new MCvScalar(25, 0, 0));
                        ScalarArray upperi = new ScalarArray(new MCvScalar(50, 255, 255 ));
                        CvInvoke.InRange(hsv_image, loweri, upperi, yellow_output);
                        // bringt nix
            */
            CvInvoke.InRange(in_image, (ScalarArray)lower, (ScalarArray)upper, yellow_output);
            out_image0ImageBox.Image = yellow_output;

            //UMat morphed_image = new UMat(yellow_output.Size, DepthType.Cv8U, 1);
            Image<Gray, Byte> morphed_image = new Image<Gray, Byte>(yellow_output.Size);
            CvInvoke.MorphologyEx(yellow_output, morphed_image, MorphOp.Close, new Mat(), new Point(-1, -1), 7, BorderType.Default, new MCvScalar(255, 255, 255));
            //CvInvoke.Dilate(yellow_output, morphed_image, null, new Point(-1, -1), 2, BorderType.Constant, new MCvScalar(255, 255, 255));
            //out_image1ImageBox.Image = morphed_image;

            //UMat canny_output = new UMat(morphed_image.Size, DepthType.Cv8U, 1);
            Image<Gray, Byte> canny_output = new Image<Gray, Byte>(morphed_image.Size);

            // AdaptiveThreshold bringt nichts, weil InRange schon ein Binary Image liefert
            //CvInvoke.AdaptiveThreshold(morphed_image, canny_output, 50, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 3, 5);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            using (VectorOfVectorOfPoint boxes = new VectorOfVectorOfPoint())
            using (Mat hierarchy = new Mat())
            {
                CvInvoke.Canny(morphed_image, canny_output, 30, 60, 3, true);
                out_image1ImageBox.Image = canny_output;

                //UMat canny_output_mod = new UMat();
                Image<Gray, Byte> canny_output_mod = new Image<Gray, Byte>(canny_output.Bitmap);
                CvInvoke.FindContours(canny_output_mod, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple, new Point(0, 0));

                double contourAreaMin = (int)numericUpDown_contourAreaMin.Value; // hullAreasAvg(contours); // // ca 14*14 = 196 nur für 640*480 !! 
                double contourAreaMax = (int)numericUpDown_contourAreaMax.Value; // damit der Gesamtrahmen gefiltert wird

                /*                RotatedRect rrect;
                                VectorOfVectorOfPoint hulls = new VectorOfVectorOfPoint(contours.Size);

                                //nur zum malen
                                for (int i = 0; i < contours.Size; i++)
                                {
                                    double contourArea = CvInvoke.ContourArea(contours[i], false);
                                    CvInvoke.ConvexHull(contours[i], hulls[i]);
                                    double hullArea = CvInvoke.ContourArea(hulls[i], false);
                                    //if (hullArea >= contourAreaMin)
                                    if (contourArea >= contourAreaMin)
                                    {
                                        CvInvoke.DrawContours(canny_output, contours, i, new MCvScalar(120));
                                        rrect = CvInvoke.MinAreaRect(contours[i]);
                                        double heightWidthRatio = rrect.Size.Height / rrect.Size.Width;
                                        Point[] polyPoints = { Point.Truncate(rrect.GetVertices()[0]), Point.Truncate(rrect.GetVertices()[1]), Point.Truncate(rrect.GetVertices()[2]), Point.Truncate(rrect.GetVertices()[3]) };
                                        //canny_output.Draw(polyPoints, new Gray(160), 2);
                                        //canny_output.Draw(i.ToString(), Point.Truncate(rrect.Center), FontFace.HersheyComplexSmall, 1, new Gray(180));
                                        //canny_output.Draw(heightWidthRatio.ToString("0.00"), Point.Truncate(rrect.Center), FontFace.HersheyComplexSmall, 1, new Gray(180));
                                    }
                                }
                */

                List<RotatedRect> rrects = new List<RotatedRect>();
                List<int> iListFoundContours = new List<int>();
                int xdist = Math.Abs(in_image.Size.Width / 4);
                int ydist = Math.Abs(in_image.Size.Height / 4);

                // funktion zum suchen von rrects mit filtern von uebereinander, input contours,hullAreaAvg, xdist, ydist -  output rrects, rechteckContours
                rrects = rotRectsGtAreaEliminateSameCentered(contours, contourAreaMin, contourAreaMax, xdist, ydist, iListFoundContours);

                //wenn genau 4 rrect erkannt wurden, clockwise sortieren
                List<RotatedRect> rrectsClockwiseSort = new List<RotatedRect>();
                if (rrects.Count == 4)
                {
                    rrectsClockwiseSort = quadRotRectClockwise(rrects);

                    int colordelta = Math.Abs(255 / iListFoundContours.Count);
                    //Image<Bgr,Byte> fiducials_output = new Image<Bgr, byte>(canny_output.Size);
                    //Draw result
                    for (int i = 0; i < iListFoundContours.Count; i++)
                    {
                        MCvScalar color = new MCvScalar((255 - (i * colordelta)), 0, (i * colordelta));
                        //von rot nach blau
                        CvInvoke.DrawContours(in_image, contours, iListFoundContours.ElementAt(i), color, 1, LineType.EightConnected, hierarchy, 0, new Point(0, 0));
                        CvInvoke.DrawContours(canny_output, contours, iListFoundContours.ElementAt(i), new MCvScalar(120));
                        canny_output.Draw(iListFoundContours.ElementAt(i).ToString(), Point.Truncate(rrectsClockwiseSort[i].Center), FontFace.HersheyComplexSmall, 1, new Gray(180));
                        // mitte der Rechtecke fèr perspectiveTransform
                        Cross2DF mitte = new Cross2DF(rrectsClockwiseSort[i].Center, 10, 10);

                        in_image.Draw(mitte, new Bgr(0, 255, 0), 1);
                        in_image.Draw(i.ToString(), Point.Truncate(rrectsClockwiseSort[i].Center), FontFace.HersheyComplexSmall, 1, new Bgr(0, 255, 0));
                    }

                    PointF[] srcTransfRect = new PointF[4] { rrectsClockwiseSort[0].Center, rrectsClockwiseSort[1].Center, rrectsClockwiseSort[2].Center, rrectsClockwiseSort[3].Center };
                    transfMat = CvInvoke.GetPerspectiveTransform(srcTransfRect, dstTransfRect);

                    CvInvoke.WarpPerspective(in_image, transform_output, transfMat, transform_output.Size);

                }
            }

            return transfMat;
        }

        string tileCenterLines = "X, Y";

        public void DetectFiducialPointsPerspectiveMat()
        {
            if (fileNameTextBox.Text != String.Empty)
            {
                //Load the image from file and resize it for display
                Image<Bgr, Byte> in_image = new Image<Bgr, Byte>(fileNameTextBox.Text); //.Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
                originalImageBox.Image = in_image;

                //                MCvScalar bgrFiltLower = new MCvScalar(71, 128, 139);// aus Gimp
                //                MCvScalar bgrFiltUpper = new MCvScalar(183, 208, 219);
                //                MCvScalar bgrFiltLower = new MCvScalar(90, 152, 173); //BGR aus Excel
                //                MCvScalar bgrFiltUpper = new MCvScalar(165, 201, 206);
                //                MCvScalar bgrFiltLower = new MCvScalar(90, 145, 165); //median +-30 aus Excel
                //                MCvScalar bgrFiltUpper = new MCvScalar(150, 205, 225);
                //                MCvScalar bgrFiltLower = new MCvScalar(81, 138, 149);// aus Gimp +- 10
                //                MCvScalar bgrFiltUpper = new MCvScalar(173, 198, 209);

                int lower_blue = (int)numericUpDown_lower_blue.Value;
                int upper_blue = (int)numericUpDown_upper_blue.Value;
                int lower_green = (int)numericUpDown_lower_green.Value;
                int upper_green = (int)numericUpDown_upper_green.Value;
                int lower_red = (int)numericUpDown_lower_red.Value;
                int upper_red = (int)numericUpDown_upper_red.Value;
                MCvScalar bgrFiltLower = new MCvScalar(lower_blue, lower_green, lower_red);
                MCvScalar bgrFiltUpper = new MCvScalar(upper_blue, upper_green, upper_red);


                int widthTransfmmMal10 =  (int)numericUpDown_widthTransfMmMal10.Value ;
                int heightTransfmmMal10 = (int)numericUpDown_heightTransfMmMal10.Value;

                Image<Bgr, byte> transform_output = new Image<Bgr, byte>(widthTransfmmMal10, heightTransfmmMal10);
                Mat transfMat = new Mat();
                transfMat = detectFiducialPointsPerspectiveMat(in_image, transform_output, bgrFiltLower, bgrFiltUpper, widthTransfmmMal10, heightTransfmmMal10);

                out_image2ImageBox.Image = transform_output;
                List<RotatedRect> tileRrectsL = new List<RotatedRect>();
                tileRrectsL = detectTilesCenter(transform_output);

                tileCenterLines = "X, Y, Angle" + Environment.NewLine ;
                string tileCenterLine;
                for (int i = 0; i < tileRrectsL.Count; i++)
                {
                    tileCenterLine = tileRrectsL[i].Center.X.ToString("0") + ", " + tileRrectsL[i].Center.Y.ToString("0") + ", " 
                        + tileRrectsL[i].Angle.ToString("0.00") + Environment.NewLine;
                    tileCenterLines += tileCenterLine;
                }
                
            }
        }

        public List<RotatedRect> detectTilesCenter(Image<Bgr, Byte> plainMmMal10_image)
        {
            List<RotatedRect> tileRrects = new List<RotatedRect>(); //return

            // die Fiducial sind 8x8mm mit Punkt in der Mitte // also bischen mehr als 8mm/2 nehmen -> 50
            Rectangle rectROI = new Rectangle(50, 50, plainMmMal10_image.Size.Width - 100, plainMmMal10_image.Size.Height - 100);

            Image<Bgr, Byte> subPlainMmMal10 = plainMmMal10_image.GetSubRect(rectROI).Copy();
            Image<Gray, Byte> contour_output = new Image<Gray, byte>(subPlainMmMal10.Size);
            CvInvoke.CvtColor(subPlainMmMal10, contour_output, ColorConversion.Bgr2Gray);
            //CvInvoke.GaussianBlur(contour_output, contour_output, new Size(0, 0), 0);


            double threshhold = (int)numericUpDown_threshhold.Value;
            CvInvoke.Threshold(contour_output, contour_output, threshhold, 255, ThresholdType.Binary);
            CvInvoke.MorphologyEx(contour_output, contour_output, MorphOp.Open, new Mat(), new Point(-1, -1), 5, BorderType.Default, new MCvScalar(255, 255, 255));
            CvInvoke.Canny(contour_output, contour_output, 20, 40);

            out_image3ImageBox.Image = contour_output;

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(contour_output.Copy(), contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(subPlainMmMal10, contours, -1, new MCvScalar(0, 255, 0));

            double tileAreaMinPx = (double)numericUpDown_tileAreaMinPx.Value; //1500; //50*50 - Sicherheit
            double tileAreaMaxPx = (double)numericUpDown_tileAreaMaxPx.Value; //3000; //50*50 + Sicherheit

            // die tiles sind ca 5mm, xdist = 60, nach Transformation nach subPlainMMMal10 sind 10Pixel = 1mm 
            int xdist = 60;
            int ydist = 60;

            List<RotatedRect> rrects = new List<RotatedRect>();
            List<int> iListFoundContours = new List<int>();

            // funktion zum suchen von rrects mit filtern von uebereinander, input contours,hullAreaAvg, xdist, ydist -  output rrects, rechteckContours
            rrects = rotRectsGtAreaEliminateSameCentered(contours, tileAreaMinPx, tileAreaMaxPx, xdist, ydist, iListFoundContours);

            for (int i = 0; i < iListFoundContours.Count; i++)
            {
                CvInvoke.DrawContours(subPlainMmMal10, contours, iListFoundContours.ElementAt(i), new MCvScalar(0, 255, 255));
                Cross2DF mitte = new Cross2DF(rrects[i].Center, 10, 10);
                
                tileRrects.Add(rrects[i]);

                Point[] rr = { Point.Truncate(rrects[i].GetVertices()[0]), Point.Truncate(rrects[i].GetVertices()[1]), Point.Truncate(rrects[i].GetVertices()[2]), Point.Truncate(rrects[i].GetVertices()[3]) };
                subPlainMmMal10.DrawPolyline(rr, true, new Bgr(0, 255, 255));

                subPlainMmMal10.Draw(mitte, new Bgr(0, 255, 255), 2);
            }

            out_image4ImageBox.Image = subPlainMmMal10;


            return tileRrects;
        }

        public void DetectTile()
        {
            if (fileNameTextBox.Text != String.Empty)
            {
                //Load the image from file and resize it for display
                Image<Bgr, Byte> in_image = new Image<Bgr, Byte>(fileNameTextBox.Text); //.Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
                originalImageBox.Image = in_image;

                detectTilesCenter(in_image);
            }
        }

        public void saveTilePositions()
        {

        }

        public void PerformShapeDetection()
        {
            if (fileNameTextBox.Text != String.Empty)
            {
                StringBuilder msgBuilder = new StringBuilder("Performance: ");

                //Load the image from file and resize it for display
                Image<Bgr, Byte> img =
                   new Image<Bgr, byte>(fileNameTextBox.Text)
                   .Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

                //Convert the image to grayscale and filter out the noise
                UMat uimage = new UMat();
                CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

                //use image pyr to remove noise
                UMat pyrDown = new UMat();
                CvInvoke.PyrDown(uimage, pyrDown);
                CvInvoke.PyrUp(pyrDown, uimage);

                //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

                #region circle detection
                Stopwatch watch = Stopwatch.StartNew();
                double cannyThreshold = 180.0;
                double circleAccumulatorThreshold = 120;
                CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

                watch.Stop();
                msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
                #endregion

                #region Canny and edge detection
                watch.Reset(); watch.Start();
                double cannyThresholdLinking = 120.0;
                UMat cannyEdges = new UMat();
                CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

                LineSegment2D[] lines = CvInvoke.HoughLinesP(
                   cannyEdges,
                   1, //Distance resolution in pixel-related units
                   Math.PI / 45.0, //Angle resolution measured in radians.
                   20, //threshold
                   30, //min Line width
                   10); //gap between lines

                watch.Stop();
                msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));
                #endregion

                #region Find triangles and rectangles
                watch.Reset(); watch.Start();
                List<Triangle2DF> triangleList = new List<Triangle2DF>();
                List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++)
                    {
                        using (VectorOfPoint contour = contours[i])
                        using (VectorOfPoint approxContour = new VectorOfPoint())
                        {
                            CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                            if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                            {
                                if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                                {
                                    Point[] pts = approxContour.ToArray();
                                    triangleList.Add(new Triangle2DF(
                                       pts[0],
                                       pts[1],
                                       pts[2]
                                       ));
                                } else if (approxContour.Size == 4) //The contour has 4 vertices.
                                {
                                    #region determine if all the angles in the contour are within [80, 100] degree
                                    bool isRectangle = true;
                                    Point[] pts = approxContour.ToArray();
                                    LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                    for (int j = 0; j < edges.Length; j++)
                                    {
                                        double angle = Math.Abs(
                                           edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                        if (angle < 80 || angle > 100)
                                        {
                                            isRectangle = false;
                                            break;
                                        }
                                    }
                                    #endregion

                                    if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                }
                            }
                        }
                    }
                }

                watch.Stop();
                msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; ", watch.ElapsedMilliseconds));
                #endregion

                originalImageBox.Image = img;
                this.Text = msgBuilder.ToString();

                #region draw triangles and rectangles
                Mat triangleRectangleImage = new Mat(img.Size, DepthType.Cv8U, 3);
                triangleRectangleImage.SetTo(new MCvScalar(0));
                foreach (Triangle2DF triangle in triangleList)
                {
                    CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(triangle.GetVertices(), Point.Round), true, new Bgr(Color.DarkBlue).MCvScalar, 2);
                }
                foreach (RotatedRect box in boxList)
                {
                    CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(box.GetVertices(), Point.Round), true, new Bgr(Color.DarkOrange).MCvScalar, 2);
                }

                out_image0ImageBox.Image = triangleRectangleImage;
                #endregion

                #region draw circles
                Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3);
                circleImage.SetTo(new MCvScalar(0));
                foreach (CircleF circle in circles)
                    CvInvoke.Circle(circleImage, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Brown).MCvScalar, 2);

                out_image1ImageBox.Image = circleImage;
                #endregion

                #region draw lines
                Mat lineImage = new Mat(img.Size, DepthType.Cv8U, 3);
                lineImage.SetTo(new MCvScalar(0));
                foreach (LineSegment2D line in lines)
                    CvInvoke.Line(lineImage, line.P1, line.P2, new Bgr(Color.Green).MCvScalar, 2);

                out_image2ImageBox.Image = lineImage;
                #endregion
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //PerformShapeDetection();
            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_Global_ValueChanged(object sender, EventArgs e)
        {
            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                fileNameTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void saveTilePositionsButton_Click(object sender, EventArgs e)
        {
            string suggestfileName = fileNameTextBox.Text;
            saveFileDialog1.FileName = suggestfileName.Substring(0, suggestfileName.Length-3) + "txt";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                string saveFileTilePositions = saveFileDialog1.FileName ;
                System.IO.File.WriteAllText(saveFileTilePositions, tileCenterLines);

            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown_lower_blue_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_blue.Value > this.numericUpDown_upper_blue.Value)
            {
                this.numericUpDown_upper_blue.Value = this.numericUpDown_lower_blue.Value;
            }
            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_upper_blue_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_blue.Value > this.numericUpDown_upper_blue.Value)
            {
                this.numericUpDown_upper_blue.Value = this.numericUpDown_lower_blue.Value;
            }

            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_lower_green_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_green.Value > this.numericUpDown_upper_green.Value)
            {
                this.numericUpDown_upper_green.Value = this.numericUpDown_lower_green.Value;
            }
            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_upper_green_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_green.Value > this.numericUpDown_upper_green.Value)
            {
                this.numericUpDown_upper_green.Value = this.numericUpDown_lower_green.Value;
            }

            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_lower_red_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_red.Value > this.numericUpDown_upper_red.Value)
            {
                this.numericUpDown_upper_red.Value = this.numericUpDown_lower_red.Value;
            }
            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_upper_red_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_red.Value > this.numericUpDown_upper_red.Value)
            {
                this.numericUpDown_upper_red.Value = this.numericUpDown_lower_red.Value;
            }

            //DetectTile();
            DetectFiducialPointsPerspectiveMat();
        }

        private void numericUpDown_contourAreaMin_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_contourAreaMin.Value > this.numericUpDown_contourAreaMax.Value)
            {
                this.numericUpDown_contourAreaMax.Value = this.numericUpDown_contourAreaMin.Value;
            }

        }

        private void numericUpDown_contourAreaMax_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_contourAreaMin.Value > this.numericUpDown_contourAreaMax.Value)
            {
                this.numericUpDown_contourAreaMax.Value = this.numericUpDown_contourAreaMin.Value;
            }
        }
   
    }


}

