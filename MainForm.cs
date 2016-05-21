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

        const int CV_QR_NORTH = 0;
        const int CV_QR_EAST = 1;
        const int CV_QR_SOUTH = 2;
        const int CV_QR_WEST = 3;


        public Mat detectQRPointsPerspectiveMat(Image<Bgr, Byte> in_image, IImage transform_output, MCvScalar lower, MCvScalar upper, int width, int height)
        {
            Mat transfMat = new Mat(); //return
            PointF[] dstTransfRect = new PointF[4] { new Point(0, height), new Point(0, 0), new Point(width, 0), new Point(width, height) };

            // QR hierarchy erkennen http://dsynflo.blogspot.ch/2014/10/opencv-qr-code-detection-and-extraction.html

            //UMat yellow_output = new UMat(in_image.Size, DepthType.Cv8U, 1);
            //geht mit UMat nicht, da Canny einen Bug hat http://code.opencv.org/issues/4120

            Image<Gray, Byte> pyr_image = new Image<Gray, Byte>(in_image.Size);
            CvInvoke.PyrUp(in_image, pyr_image);
            CvInvoke.PyrDown(pyr_image, pyr_image);
            

            //UMat canny_output = new UMat(morphed_image.Size, DepthType.Cv8U, 1);
            Image<Gray, Byte> canny_output = new Image<Gray, Byte>(in_image.Size);

            // AdaptiveThreshold bringt nichts, weil InRange schon ein Binary Image liefert
            //CvInvoke.AdaptiveThreshold(morphed_image, canny_output, 50, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 3, 5);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            using (VectorOfVectorOfPoint boxes = new VectorOfVectorOfPoint())
            using (Mat hierarchy = new Mat())
            {
                CvInvoke.Canny(in_image, canny_output, 30, 60, 3, true);
                out_image0ImageBox.Image = canny_output;

                //UMat canny_output_mod = new UMat();
                Image<Gray, Byte> canny_output_mod = new Image<Gray, Byte>(canny_output.Bitmap);
                //CvInvoke.FindContours(canny_output_mod, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple, new Point(0, 0));
                int[,] hierar;
                hierar = CvInvoke.FindContourTree(canny_output_mod, contours, ChainApproxMethod.ChainApproxSimple);
                //ArraySegment<int> hierar0 = new ArraySegment<int>(hierar.); 
                // testen ob eine contour 8 weitere untercontouren hat
                // erst unterste contour finden hierarchy[i][1] = -1, dann schauen bis hierarchy[i][0] in hierarchy[x][1] nicht mehr zu finden ist (sollte bei validen contours 7 mal funktionieren)
                // abbruch wenn mehr als einmal zur finden
                // oder gar nicht zu finden
                int mark = 0, A = 0, B = 0, C = 0;
                int top = 0, outlier = 0, median1 = 0, median2 = 0;
                List<int> qrFidsL = new List<int>();
                for (int i = 0; i < hierar.GetLength(0); i++)
                {
                    int actc = i;
                    int anzSubC = 0;
                    //canny_output.Draw(i.ToString(), contours[i][0], FontFace.HersheyComplexSmall, 1, new Gray(255));
                    while (hierar[actc, 2] != -1)
                    {
                        actc = hierar[actc, 2];
                        anzSubC = anzSubC + 1;
                    }
                    if (anzSubC >= 6)
                    {
                        // contours nicht in die Liste schreiben wenn schon der parent in der qrFidsL Liste ist, 
                        //was ist dann mit dem child,child?
                        int foundC = qrFidsL.Find(item => item == hierar[i, 3]);
                        if (foundC == 0)
                        {
                            qrFidsL.Add(i);
                            canny_output.Draw(i.ToString(), contours[i][0], FontFace.HersheyComplexSmall, 1, new Gray(255));

                            if (mark == 0) A = mark; //i;
                            else if (mark == 1) B = mark; //i;      // i.e., A is already found, assign current contour to B
                            else if (mark == 2) C = mark; //i;      // i.e., A and B are already found, assign current contour to C
                            mark = mark + 1;
                        }
                    }
                }

                //per distance den A, B, C herausfinden
                List<MCvMoments> momentL = new List<MCvMoments>();
                List<PointF> massCenterL = new List<PointF>();
                for (int i = 0; i < qrFidsL.Count; i++)
                {
                    momentL.Add(CvInvoke.Moments(contours[qrFidsL.ElementAt(i)], false));
                    //massCenterL[i] = new PointF((float)(momentL[i].M10 / momentL[i].M00), (float)(momentL[i].M01 / momentL[i].M00));
                    massCenterL.Add(new PointF((float)momentL[i].GravityCenter.X, (float)momentL[i].GravityCenter.Y));
                }

                // jetzt sollten nur exakt 3 Fids existieren
                if (qrFidsL.Count == 3)
                {
                    float distAB = cv_distance(massCenterL[0], massCenterL[1]);
                    float distCA = cv_distance(massCenterL[0], massCenterL[2]);
                    float distBC = cv_distance(massCenterL[1], massCenterL[2]);

                    if (distAB > distBC && distAB > distCA)
                    {
                        outlier = C; median1 = A; median2 = B;
                    }
                    else if (distCA > distAB && distCA > distBC)
                    {
                        outlier = B; median1 = A; median2 = C;
                    }
                    else if (distBC > distAB && distBC > distCA)
                    {
                        outlier = A; median1 = B; median2 = C;
                    }

                    top = outlier;

                    canny_output.Draw(" A:" + A.ToString(), contours[qrFidsL.ElementAt(A)][0], FontFace.HersheyComplexSmall, 2, new Gray(255));
                    canny_output.Draw(" B:" + B.ToString(), contours[qrFidsL.ElementAt(B)][0], FontFace.HersheyComplexSmall, 2, new Gray(255));
                    canny_output.Draw(" C:" + C.ToString(), contours[qrFidsL.ElementAt(C)][0], FontFace.HersheyComplexSmall, 2, new Gray(255));

                    float dist = 0;
                    float slope = 0;
                    int align = 0;
                    int orientation = 0;
                    int right = 0;
                    int bottom = 0;
                    dist = cv_lineEquation(massCenterL[median1], massCenterL[median2], massCenterL[outlier]);  // Get the Perpendicular distance of the outlier from the longest side			
                    slope = cv_lineSlope(massCenterL[median1], massCenterL[median2], ref align);      // Also calculate the slope of the longest side

                    // Now that we have the orientation of the line formed median1 & median2 and we also have the position of the outlier w.r.t. the line
                    // Determine the 'right' and 'bottom' markers

                    if (align == 0)
                    {
                        bottom = median1;
                        right = median2;
                    }
                    else if (slope < 0 && dist < 0)     // Orientation - North
                    {
                        bottom = median1;
                        right = median2;
                        orientation = CV_QR_NORTH;
                    }
                    else if (slope > 0 && dist < 0)     // Orientation - East
                    {
                        right = median1;
                        bottom = median2;
                        orientation = CV_QR_EAST;
                    }
                    else if (slope < 0 && dist > 0)     // Orientation - South			
                    {
                        right = median1;
                        bottom = median2;
                        orientation = CV_QR_SOUTH;
                    }

                    else if (slope > 0 && dist > 0)     // Orientation - West
                    {
                        bottom = median1;
                        right = median2;
                        orientation = CV_QR_WEST;
                    }


                    //--------
                    // To ensure any unintended values do not sneak up when QR code is not present
//                    float area_top, area_right, area_bottom;

                    if (top < contours.Size && right < contours.Size && bottom < contours.Size && CvInvoke.ContourArea(contours[qrFidsL.ElementAt(top)]) > 10 && CvInvoke.ContourArea(contours[qrFidsL.ElementAt(right)]) > 10 && CvInvoke.ContourArea(contours[qrFidsL.ElementAt(bottom)]) > 10)
                    {

                        VectorOfPointF L = new VectorOfPointF();
                        VectorOfPointF M = new VectorOfPointF();
                        VectorOfPointF O = new VectorOfPointF();
                        VectorOfPointF tempL = new VectorOfPointF();
                        VectorOfPointF tempM = new VectorOfPointF();
                        VectorOfPointF tempO = new VectorOfPointF();
                        PointF N = new PointF();

                        VectorOfPointF src = new VectorOfPointF();
                        VectorOfPointF dst = new VectorOfPointF();       // src - Source Points basically the 4 end co-ordinates of the overlay image
                                                                         // dst - Destination Points to transform overlay image	

                        Mat warp_matrix;

                        cv_getVertices(contours, qrFidsL.ElementAt(top), slope, ref tempL);
                        cv_getVertices(contours, qrFidsL.ElementAt(right), slope, ref tempM);
                        cv_getVertices(contours, qrFidsL.ElementAt(bottom), slope, ref tempO);

                        cv_updateCornerOr(orientation, tempL, ref L);           // Re-arrange marker corners w.r.t orientation of the QR code
                        cv_updateCornerOr(orientation, tempM, ref M);           // Re-arrange marker corners w.r.t orientation of the QR code
                        cv_updateCornerOr(orientation, tempO, ref O);           // Re-arrange marker corners w.r.t orientation of the QR code

                        bool iflag = getIntersectionPoint(M[1], M[2], O[3], O[2], ref N);

                        PointF[] srcA = { L[0], M[1], N, O[3] };
                        src.Push(srcA);

                        PointF[] dstA = { new PointF(0, 0), new PointF(transform_output.Size.Width, 0), new PointF(transform_output.Size.Width, transform_output.Size.Height), new PointF(0, transform_output.Size.Height) };
                        dst.Push(dstA);

                        if (src.Size == 4 && dst.Size == 4)         // Failsafe for WarpMatrix Calculation to have only 4 Points with src and dst
                        {
                            transfMat = CvInvoke.GetPerspectiveTransform(src, dst);
                            CvInvoke.WarpPerspective(in_image, transform_output, transfMat, transform_output.Size);
                            //                           CvInvoke.CopyMakeBorder(transform_output, qr, 10, 10, 10, 10, BORDER_CONSTANT, Scalar(255, 255, 255));

                            //                            CvInvoke.CvtColor(qr, qr_gray, CV_RGB2GRAY);
                            //                            CvInvoke.Threshold(qr_gray, qr_thres, 127, 255, CV_THRESH_BINARY);

                            //threshold(qr_gray, qr_thres, 0, 255, CV_THRESH_OTSU);
                            //for( int d=0 ; d < 4 ; d++){	src.pop_back(); dst.pop_back(); }
                        }
                    }
                }
            }            
            out_image1ImageBox.Image = transform_output;

            return transfMat;
        }

        float cv_distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(Math.Abs(a.X - b.X), 2) + Math.Pow(Math.Abs(a.Y - b.Y), 2));
        }

        // Function: Perpendicular Distance of a Point J from line formed by Points L and M; Equation of the line ax+by+c=0
        // Description: Given 3 points, the function derives the line quation of the first two points,
        //	  calculates and returns the perpendicular distance of the the 3rd point from this line.

        float cv_lineEquation(PointF L, PointF M, PointF J)
        {
            float a, b, c, pdist;

            a = -((M.Y - L.Y) / (M.X - L.X));
            b = 1;
            c = (((M.Y - L.Y) / (M.X - L.X)) * L.X) - L.Y;

            // Now that we have a, b, c from the equation ax + by + c, time to substitute (x,y) by values from the Point J

            pdist = (a * J.X + (b * J.Y) + c) / (float)Math.Sqrt((a * a) + (b * b));
            return pdist;
        }

        // Function: Slope of a line by two Points L and M on it; Slope of line, S = (x1 -x2) / (y1- y2)
        // Description: Function returns the slope of the line formed by given 2 points, the alignement flag
        //	  indicates the line is vertical and the slope is infinity.

        float cv_lineSlope(PointF L, PointF M, ref int alignement)
        {
            float dx, dy;
            dx = M.X - L.X;
            dy = M.Y - L.Y;

            if (dy != 0)
            {
                alignement = 1;
                return (dy / dx);
            }
            else                // Make sure we are not dividing by zero; so use 'alignement' flag
            {
                alignement = 0;
                return 0;
            }
        }

        // Function: Routine to calculate 4 Corners of the Marker in Image Space using Region partitioning
        // Theory: OpenCV Contours stores all points that describe it and these points lie the perimeter of the polygon.
        //	The below function chooses the farthest points of the polygon since they form the vertices of that polygon,
        //	exactly the points we are looking for. To choose the farthest point, the polygon is divided/partitioned into
        //	4 regions equal regions using bounding box. Distance algorithm is applied between the centre of bounding box
        //	every contour point in that region, the farthest point is deemed as the vertex of that region. Calculating
        //	for all 4 regions we obtain the 4 corners of the polygon ( - quadrilateral).
        void cv_getVertices(VectorOfVectorOfPoint contours, int c_id, float slope, ref VectorOfPointF quad)
        {
            Rectangle box;
            box = CvInvoke.BoundingRectangle(contours[c_id]);

            PointF M0 = new PointF();
            PointF M1 = new PointF();
            PointF M2 = new PointF();
            PointF M3 = new PointF();
            PointF A, B, C, D, W, X, Y, Z;

            A = box.Location;  //box.tl(); 
            //B.X = box.Right; // box.br().x;
            //B.Y = box.Left; // box.tl().y;
            B = new PointF(box.Right, box.Left);
            C = new PointF(box.Right, box.Bottom);// box.br();
            //D.X = box.Top ; //box.tl().x;
            //D.Y = box.Bottom ; //box.br().y;
            D = new PointF(box.Top, box.Bottom);


            //W.x = (A.x + B.x) / 2;
            //W.y = A.y;
            W = new PointF((A.X + B.X) / 2, A.Y);

            //X.x = B.x;
            //X.y = (B.y + C.y) / 2;
            X = new PointF(B.X, (B.Y + C.Y) / 2);

            //Y.x = (C.x + D.x) / 2;
            //Y.y = C.y;
            Y = new PointF((C.X + D.X) / 2, C.Y);

            //Z.x = D.x;
            //Z.y = (D.y + A.y) / 2;
            Z = new PointF(D.X, (D.Y + A.Y) / 2);

            float[] dmax = new float[4];
            dmax[0] = 0;
            dmax[1] = 0;
            dmax[2] = 0;
            dmax[3] = 0;

            float pd1 = 0;
            float pd2 = 0;

            if (slope > 5 || slope < -5)
            {

                for (int i = 0; i < contours[c_id].Size; i++)
                {
                    pd1 = cv_lineEquation(C, A, contours[c_id][i]); // Position of point w.r.t the diagonal AC 
                    pd2 = cv_lineEquation(B, D, contours[c_id][i]); // Position of point w.r.t the diagonal BD

                    if ((pd1 >= 0.0) && (pd2 > 0.0))
                    {
                        cv_updateCorner(contours[c_id][i], W, ref dmax[1], ref M1);
                    }
                    else if ((pd1 > 0.0) && (pd2 <= 0.0))
                    {
                        cv_updateCorner(contours[c_id][i], X, ref dmax[2], ref M2);
                    }
                    else if ((pd1 <= 0.0) && (pd2 < 0.0))
                    {
                        cv_updateCorner(contours[c_id][i], Y, ref dmax[3], ref M3);
                    }
                    else if ((pd1 < 0.0) && (pd2 >= 0.0))
                    {
                        cv_updateCorner(contours[c_id][i], Z, ref dmax[0], ref M0);
                    }
                    else
                        continue;
                }
            }
            else
            {
                int halfx = (int)((A.X + B.X) / 2);
                int halfy = (int)((A.Y + D.Y) / 2);

                for (int i = 0; i < contours[c_id].Size; i++)
                {
                    if ((contours[c_id][i].X < halfx) && (contours[c_id][i].Y <= halfy))
                    {
                        cv_updateCorner(contours[c_id][i], C, ref dmax[2], ref M0);
                    }
                    else if ((contours[c_id][i].X >= halfx) && (contours[c_id][i].Y < halfy))
                    {
                        cv_updateCorner(contours[c_id][i], D, ref dmax[3], ref M1);
                    }
                    else if ((contours[c_id][i].X > halfx) && (contours[c_id][i].Y >= halfy))
                    {
                        cv_updateCorner(contours[c_id][i], A, ref dmax[0], ref M2);
                    }
                    else if ((contours[c_id][i].X <= halfx) && (contours[c_id][i].Y > halfy))
                    {
                        cv_updateCorner(contours[c_id][i], B, ref dmax[1], ref M3);
                    }
                }
            }

            PointF[] quadA = { M0, M1, M2, M3 };
            quad.Push(quadA);

        }

        // Function: Compare a point if it more far than previously recorded farthest distance
        // Description: Farthest Point detection using reference point and baseline distance
        void cv_updateCorner(PointF P, PointF reference , ref float baseline, ref PointF corner)
        {
            float temp_dist;
            temp_dist = cv_distance(P, reference);

            if (temp_dist > baseline)
            {
                baseline = temp_dist;           // The farthest distance is the new baseline
                corner = P;                     // P is now the farthest point
            }

        }

        // Function: Sequence the Corners wrt to the orientation of the QR Code
        void cv_updateCornerOr(int orientation, VectorOfPointF IN, ref VectorOfPointF OUT)
        {
            PointF M0 = new PointF();
            PointF M1 = new PointF();
            PointF M2 = new PointF();
            PointF M3 = new PointF(); 
            if (orientation == CV_QR_NORTH)
            {
                M0 = IN[0];
                M1 = IN[1];
                M2 = IN[2];
                M3 = IN[3];
            }
            else if (orientation == CV_QR_EAST)
            {
                M0 = IN[1];
                M1 = IN[2];
                M2 = IN[3];
                M3 = IN[0];
            }
            else if (orientation == CV_QR_SOUTH)
            {
                M0 = IN[2];
                M1 = IN[3];
                M2 = IN[0];
                M3 = IN[1];
            }
            else if (orientation == CV_QR_WEST)
            {
                M0 = IN[3];
                M1 = IN[0];
                M2 = IN[1];
                M3 = IN[2];
            }

            PointF[] OUTA = { M0, M1, M2, M3 };
            OUT.Push(OUTA);
 
        }

        // Function: Get the Intersection Point of the lines formed by sets of two points
        bool getIntersectionPoint(PointF a1, PointF a2, PointF b1, PointF b2, ref PointF intersection)
        {
            PointF p = a1;
            PointF q = b1;
            PointF r = new PointF(a2.X - a1.X, a2.Y - a1.Y);
            PointF s = new PointF(b2.X - b1.X, b2.Y - b1.Y);

            if (cross(r, s) == 0) { return false; }

            float t = cross(new PointF(q.X - p.X, q.Y - p.Y), s) / cross(r, s);

            intersection = PointF.Add(p,new SizeF(t * r.X, t * r.Y));
            return true;
        }

        float cross(PointF v1, PointF v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        string tileCenterLines = "";

        public void runDetectTile()
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
                //transfMat = detectFiducialPointsPerspectiveMat(in_image, transform_output, bgrFiltLower, bgrFiltUpper, widthTransfmmMal10, heightTransfmmMal10);
                transfMat = detectQRPointsPerspectiveMat(in_image, transform_output, bgrFiltLower, bgrFiltUpper, widthTransfmmMal10, heightTransfmmMal10);

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
            runDetectTile();
        }

        private void numericUpDown_Global_ValueChanged(object sender, EventArgs e)
        {
            //DetectTile();
            runDetectTile();
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



        private void numericUpDown_lower_blue_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_blue.Value > this.numericUpDown_upper_blue.Value)
            {
                this.numericUpDown_upper_blue.Value = this.numericUpDown_lower_blue.Value;
            }
            //DetectTile();
            runDetectTile();
        }

        private void numericUpDown_upper_blue_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_blue.Value > this.numericUpDown_upper_blue.Value)
            {
                this.numericUpDown_upper_blue.Value = this.numericUpDown_lower_blue.Value;
            }

            //DetectTile();
            runDetectTile();
        }

        private void numericUpDown_lower_green_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_green.Value > this.numericUpDown_upper_green.Value)
            {
                this.numericUpDown_upper_green.Value = this.numericUpDown_lower_green.Value;
            }
            //DetectTile();
            runDetectTile();
        }

        private void numericUpDown_upper_green_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_green.Value > this.numericUpDown_upper_green.Value)
            {
                this.numericUpDown_upper_green.Value = this.numericUpDown_lower_green.Value;
            }

            //DetectTile();
            runDetectTile();
        }

        private void numericUpDown_lower_red_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_red.Value > this.numericUpDown_upper_red.Value)
            {
                this.numericUpDown_upper_red.Value = this.numericUpDown_lower_red.Value;
            }
            //DetectTile();
            runDetectTile();
        }

        private void numericUpDown_upper_red_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown_lower_red.Value > this.numericUpDown_upper_red.Value)
            {
                this.numericUpDown_upper_red.Value = this.numericUpDown_lower_red.Value;
            }

            //DetectTile();
            runDetectTile();
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

        private void tidi_button_Click(object sender, EventArgs e)
        {

        }
    }


}

