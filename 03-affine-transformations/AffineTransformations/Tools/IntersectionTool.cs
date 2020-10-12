using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GraphFunc.Tools
{
    class IntersectionTool : ITool//Уайлер-Атертон
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
            Console.WriteLine("started");
            bool flag = true;
            bool fin = true;
            Polygon polygon_A = new Polygon();
            int ind_A = 0;
            Polygon polygon_B = new Polygon();
            int ind_B = 0;
            List<Point> polygon_A_points = new List<Point>();
            List<Point> polygon_B_points = new List<Point>();

            //берём 2 полигона
            for (var i = 0; i < polygonContainer._polygons.Count; i++)
            {
                if (polygonContainer._polygons[i].HasPoint(point))
                    if (flag)
                    {
                        foreach (Point p in polygonContainer._polygons[i]._points)
                            polygon_A_points.Add(p);
                        polygon_A = polygonContainer._polygons[i];
                        ind_A = i;
                        flag = false;
                    }
                    else
                    {
                        foreach (Point p in polygonContainer._polygons[i]._points)
                            polygon_B_points.Add(p);
                        polygon_B = polygonContainer._polygons[i];
                        ind_B = i;
                        fin = false;
                        break;
                    }
            }
            if (!fin)
            {
                List<(Point, int)> polygon_A_points_marked = new List<(Point, int)>();
                List<(Point, int)> polygon_B_points_marked = new List<(Point, int)>();
                Console.WriteLine("pols finded");

                //для каждого полигона помечаем точки, находящиеся внутри другого
                for (var j = 0; j < polygon_A_points.Count(); j++)
                {
                    if (polygon_B.HasPoint(polygon_A_points[j]))
                        polygon_A_points_marked.Add((polygon_A_points[j], 1));
                    else
                        polygon_A_points_marked.Add((polygon_A_points[j], 0));
                }
                for (var j = 0; j < polygon_B_points.Count(); j++)
                {
                    if (polygon_A.HasPoint(polygon_B_points[j]))
                        polygon_B_points_marked.Add((polygon_B_points[j], 1));
                    else
                        polygon_B_points_marked.Add((polygon_B_points[j], 0));
                }

                //получаем стороны полигонов
                List<Edge> polygon_A_edges = new List<Edge>();
                List<Edge> polygon_B_edges = new List<Edge>();

                (Point, int) t = polygon_A_points_marked[0];
                for (var i = 1; i < polygon_A_points_marked.Count(); i++)
                {
                    polygon_A_edges.Add(new Edge(t.Item1, polygon_A_points_marked[i].Item1));
                    t = polygon_A_points_marked[i];
                }

                polygon_A_edges.Add(new Edge(t.Item1, polygon_A_points_marked[0].Item1));
                Console.WriteLine("points found");

                t = polygon_B_points_marked[0];
                for (var i = 1; i < polygon_B_points_marked.Count(); i++)
                {
                    polygon_B_edges.Add(new Edge(t.Item1, polygon_B_points_marked[i].Item1));
                    t = polygon_B_points_marked[i];
                }

                polygon_B_edges.Add(new Edge(t.Item1, polygon_B_points_marked[0].Item1));


                //для каждого полигона добавляем точки пересечений сторон
                int intersection_count = 0;
                int count;
                foreach (Edge e in polygon_A_edges)
                {
                    count = 0;
                    foreach (Edge q in polygon_B_edges)
                    {
                        PointF? pf = new PointF();
                        pf = e.Intersection(q);
                        if (pf.HasValue)
                        {
                            Console.WriteLine("int pont");
                            if (count == 0)
                            {
                                intersection_count++;
                                if (polygon_A_points_marked.Contains((e.SourceToPoint(), 0)))
                                {
                                    int ind = polygon_A_points_marked.FindIndex(x => x == (e.SourceToPoint(), 0));
                                    polygon_A_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                }
                                if (polygon_A_points_marked.Contains((e.SourceToPoint(), 1)))
                                {
                                    int ind = polygon_A_points_marked.FindIndex(x => x == (e.SourceToPoint(), 1));
                                    polygon_A_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                }
                                count++;
                            }
                            else
                            {
                                intersection_count++;
                                if (polygon_A_points_marked.Contains((e.SourceToPoint(), 0)))
                                {
                                    int ind = polygon_A_points_marked.FindIndex(x => x == (e.SourceToPoint(), 0));
                                    if (Math.Pow(pf.Value.X - e.SourceToPoint().X, 2) + Math.Pow(pf.Value.Y - e.SourceToPoint().Y, 2) > Math.Pow(polygon_A_points_marked[ind + 1].Item1.X - e.SourceToPoint().X, 2) + Math.Pow(polygon_A_points_marked[ind + 1].Item1.Y - e.SourceToPoint().Y, 2))
                                        polygon_A_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                    else
                                        polygon_A_points_marked.Insert(ind + 1, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));

                                }
                                if (polygon_A_points_marked.Contains((e.SourceToPoint(), 1)))
                                {
                                    int ind = polygon_A_points_marked.FindIndex(x => x == (e.SourceToPoint(), 1));
                                    if (Math.Pow(pf.Value.X - e.SourceToPoint().X, 2) + Math.Pow(pf.Value.Y - e.SourceToPoint().Y, 2) > Math.Pow(polygon_A_points_marked[ind + 1].Item1.X - e.SourceToPoint().X, 2) + Math.Pow(polygon_A_points_marked[ind + 1].Item1.Y - e.SourceToPoint().Y, 2))
                                        polygon_A_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                    else
                                        polygon_A_points_marked.Insert(ind + 1, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                }
                                count++;
                            }
                        }
                    }
                }
                foreach (Edge e in polygon_B_edges)
                {
                    count = 0;
                    foreach (Edge q in polygon_A_edges)
                    {
                        PointF? pf = new PointF();
                        pf = e.Intersection(q);
                        if (pf.HasValue)
                        {
                            Console.WriteLine("int pont");
                            if (count == 0)
                            {
                                intersection_count++;
                                if (polygon_B_points_marked.Contains((e.SourceToPoint(), 0)))
                                {
                                    int ind = polygon_B_points_marked.FindIndex(x => x == (e.SourceToPoint(), 0));
                                    polygon_B_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                }
                                if (polygon_B_points_marked.Contains((e.SourceToPoint(), 1)))
                                {
                                    int ind = polygon_B_points_marked.FindIndex(x => x == (e.SourceToPoint(), 1));
                                    polygon_B_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                }
                                count++;
                            }
                            else
                            {
                                intersection_count++;
                                if (polygon_B_points_marked.Contains((e.SourceToPoint(), 0)))
                                {
                                    int ind = polygon_B_points_marked.FindIndex(x => x == (e.SourceToPoint(), 0));
                                    if (Math.Pow(pf.Value.X - e.SourceToPoint().X, 2) + Math.Pow(pf.Value.Y - e.SourceToPoint().Y, 2) > Math.Pow(polygon_B_points_marked[ind + 1].Item1.X - e.SourceToPoint().X, 2) + Math.Pow(polygon_B_points_marked[ind + 1].Item1.Y - e.SourceToPoint().Y, 2))
                                        polygon_B_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                    else
                                        polygon_B_points_marked.Insert(ind + 1, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));

                                }
                                if (polygon_B_points_marked.Contains((e.SourceToPoint(), 1)))
                                {
                                    int ind = polygon_B_points_marked.FindIndex(x => x == (e.SourceToPoint(), 1));
                                    if (Math.Pow(pf.Value.X - e.SourceToPoint().X, 2) + Math.Pow(pf.Value.Y - e.SourceToPoint().Y, 2) > Math.Pow(polygon_B_points_marked[ind + 1].Item1.X - e.SourceToPoint().X, 2) + Math.Pow(polygon_B_points_marked[ind + 1].Item1.Y - e.SourceToPoint().Y, 2))
                                        polygon_B_points_marked.Insert(ind + 1 + count, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                    else
                                        polygon_B_points_marked.Insert(ind + 1, (new Point((int)pf.Value.X, (int)pf.Value.Y), 2));
                                }
                                count++;
                            }
                        }
                    }
                }
                Console.WriteLine("edges found");

                //строим  полигон пересечения
                if (intersection_count == 0)
                {
                    if (polygon_A.HasPoint(polygon_B_points[0]))
                        polygonContainer.Select(ind_B);

                    if (polygon_B.HasPoint(polygon_A_points[0]))
                        polygonContainer.Select(ind_A);
                    Console.WriteLine("no intersections");

                }
                else
                {
                    //находим точку полигона А
                    int p_index = 0;
                    for (int i = 0; i < polygon_A_points_marked.Count(); i++)
                    {
                        if (polygon_A_points_marked[i].Item2 == 2)
                        {
                            p_index = i;
                            break;
                        }
                    }
                    (Point, int) cur_p = polygon_A_points_marked[p_index];
                    (Point, int) prev_p = polygon_A_points_marked[p_index];
                    polygonContainer.AddPolygon();
                    polygonContainer.AddPoint(cur_p.Item1);
                    char check = 'A';
                    if (p_index == (polygon_A_points_marked.Count() - 1))
                        cur_p = polygon_A_points_marked[0];
                    else
                        cur_p = polygon_A_points_marked[p_index + 1];
                    if (cur_p.Item2 == 0)
                    {
                        int ind = polygon_B_points_marked.FindIndex(x => x == prev_p);
                        if (ind == (polygon_B_points_marked.Count() - 1))
                            cur_p = polygon_B_points_marked[0];
                        else
                            cur_p = polygon_B_points_marked[ind + 1];
                        check = 'B';
                    }
                    (Point, int) temp_p = prev_p;
                    Console.Write("final started ");
                    Console.Write(polygon_A_points_marked.Count());
                    Console.Write(" ");
                    Console.Write(polygon_B_points_marked.Count());
                    Console.WriteLine();


                    Console.Write(temp_p.Item1.X);
                    Console.Write(" ");
                    Console.Write(temp_p.Item1.Y);
                    Console.WriteLine();

                    //делаем обход точек пересечения
                    while (cur_p != temp_p)
                    {

                        int cX = cur_p.Item1.X;
                        Console.Write(cX);
                        Console.Write(" ");

                        int cY = cur_p.Item1.Y;
                        Console.Write(cY);
                        Console.Write(" ");
                        Console.WriteLine(" ");

                        if (cur_p.Item2 == 1)
                        {                          
                            polygonContainer.AddPoint(cur_p.Item1);

                            if (check == 'A')
                            {
                                int ind = polygon_A_points_marked.FindIndex(x => x == cur_p);
                                Console.Write("point added ");
                                Console.Write(ind);
                                Console.Write(" A ");
                                Console.Write(cur_p.Item1.X);
                                Console.Write(" ");
                                Console.Write(cur_p.Item1.Y);
                                Console.WriteLine();

                                if (ind == (polygon_A_points_marked.Count() - 1))
                                    cur_p = polygon_A_points_marked[0];
                                else
                                    cur_p = polygon_A_points_marked[ind + 1];
                            }
                            else
                            {
                                int ind = polygon_B_points_marked.FindIndex(x => x == cur_p);
                                Console.Write("point added ");
                                Console.Write(ind);
                                Console.Write(" B ");
                                Console.Write(cur_p.Item1.X);
                                Console.Write(" ");
                                Console.Write(cur_p.Item1.Y);
                                Console.WriteLine();

                                if (ind == (polygon_B_points_marked.Count() - 1))
                                    cur_p = polygon_B_points_marked[0];
                                else
                                    cur_p = polygon_B_points_marked[ind + 1];
                            }
                        }
                        else if (cur_p.Item2 == 2)
                        {

                            polygonContainer.AddPoint(cur_p.Item1);
                            if (check == 'A')
                            {
                                check = 'B';
                                int ind = polygon_B_points_marked.FindIndex(x => x == cur_p);
                                Console.Write("point added ");
                                Console.Write(ind);
                                Console.Write(" pol switched from A to B ");
                                Console.Write(cur_p.Item1.X);
                                Console.Write(" ");
                                Console.Write(cur_p.Item1.Y);
                                Console.WriteLine();

                                if (ind == (polygon_B_points_marked.Count() - 1))
                                    cur_p = polygon_B_points_marked[0];
                                else
                                    cur_p = polygon_B_points_marked[ind + 1];
                            }
                            else if (check == 'B')
                            {
                                check = 'A';
                                int ind = polygon_A_points_marked.FindIndex(x => x == cur_p);
                                Console.Write("point added ");
                                Console.Write(ind);
                                Console.Write(" pol switched from B to A ");
                                Console.Write(cur_p.Item1.X);
                                Console.Write(" ");
                                Console.Write(cur_p.Item1.Y);
                                Console.WriteLine();
                                if (ind == (polygon_A_points_marked.Count() - 1))
                                    cur_p = polygon_A_points_marked[0];
                                else
                                    cur_p = polygon_A_points_marked[ind + 1];
                            }
                        }
                    }
                }
            }
        }

        public bool CanUseInField()
            => true;

        public override string ToString()
            => "Intersection";
    }
}
