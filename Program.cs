using System;
using System.Collections.Generic;

namespace ConsoleApp4
{
     public abstract class Base
    {
        public abstract void MakeNewCenters();
        public abstract Base DeepCopy();
        public void CalculateCosts()
        {
            double cost = 0;
            costs_.Clear();
            for (int i = 0; i < clusters_.Values.Count; ++i)
            {
                foreach (var point in clusters_[i])
                {
                    cost += Math.Sqrt(Math.Pow((centers_[i].X_ - point.X_), 2) + Math.Pow((centers_[i].Y_ - point.Y_), 2));
                }
                costs_.Add(cost);
                cost = 0;
            }
        }

        public int ClosestCenter(Coordinate point)
        {
            double Closest = Double.MaxValue;
            int index = 0;
            for (int i = 0; i < centers_.Count; ++i)
            {
                if (Closest > Math.Sqrt(Math.Pow((centers_[i].X_ - point.X_), 2) + Math.Pow((centers_[i].Y_ - point.Y_), 2)))
                {
                    Closest = Math.Sqrt(Math.Pow((centers_[i].X_ - point.X_), 2) + Math.Pow((centers_[i].Y_ - point.Y_), 2));
                    index = i;
                }
            }
            return index;
        }
        public void ClearClusters()
        {
            for (int i = 0; i < clusters_.Values.Count; ++i)
            {
                clusters_[i].Clear();
            }
        }
        public void AddToCluster(int index, Coordinate point)
        {
            clusters_[index].Add(point);
        }
        public bool CompareClusters(Base current, Base previous)
        {
            if (previous.costs_.Count == 0)
            {
                foreach(var cost in current.costs_)
                {
                    initialCosts_ += cost;
                }       
            }
            double currentCost = 0;
            foreach (var cost in current.costs_)
            {
                currentCost += cost;
            }
            Console.WriteLine("     " + Math.Round(100 - (currentCost / initialCosts_ * 100),2) + "     " + Math.Round(currentCost,2));
            for (int i = 0; i < current.clusters_.Values.Count; ++i)
            {
                if (current.clusters_[i].Count != previous.clusters_[i].Count)
                {
                    return false;
                }
            }
            return true;
        }
        public List<Coordinate> centers_ { get; set; }
        public SortedDictionary<int, List<Coordinate>> clusters_ { get; set; }
        public List<double> costs_ { get; set; }
        public double initialCosts_ { get; set; }
    }
    class Kmeans : Base
    {  
        public Kmeans(List<Coordinate> centers)
        {
            centers_ = centers;
            costs_ = new List<double>();
            clusters_ = new SortedDictionary<int, List<Coordinate>>();
            for(int i = 0; i < centers.Count; ++i)
            {
                clusters_.Add(i, new List<Coordinate>());
            }
            initialCosts_ = 0;

        }
        public override Base DeepCopy()
        {
            Kmeans other = (Kmeans)this.MemberwiseClone();
            other.centers_ = new List<Coordinate>();
            foreach(var center in centers_)
            {
                other.centers_.Add(center);
            }
            other.clusters_ = new SortedDictionary<int, List<Coordinate>>();
            for(int i = 0; i < clusters_.Count; ++i)
            {
                other.clusters_.Add(i, new List<Coordinate>());
                for(int j = 0; j < clusters_[i].Count; ++j)
                {
                    other.clusters_[i].Add(clusters_[i][j]);
                }
            }
            other.costs_ = new List<double>();
            foreach (var cost in costs_)
            {
                other.costs_.Add(cost);
            }
            return other;
        }
        public override void MakeNewCenters()
        {
            double x = 0, y = 0;
            centers_.Clear();
            foreach (var cluster in clusters_.Values)
            {
                foreach(var point in cluster)
                {
                    x = x + point.X_;
                    y = y + point.Y_;
                }
                x /= cluster.Count;
                y /= cluster.Count;
                centers_.Add(new Coordinate(x, y));
                x = 0;
                y = 0;
            }
        }
    }
    class Kmedoids : Base
    {
        public Kmedoids(List<Coordinate> centers)
        {
            centers_ = centers;
            costs_ = new List<double>();
            clusters_ = new SortedDictionary<int, List<Coordinate>>();
            for (int i = 0; i < centers.Count; ++i)
            {
                clusters_.Add(i, new List<Coordinate>());
            }
            initialCosts_ = 0;
        }
        public override Base DeepCopy()
        {
            Kmedoids other = (Kmedoids)this.MemberwiseClone();
            other.centers_ = new List<Coordinate>();
            foreach (var c in centers_)
            {
                other.centers_.Add(c);
            }
            other.clusters_ = new SortedDictionary<int, List<Coordinate>>();
            for (int i = 0; i < clusters_.Count; ++i)
            {
                other.clusters_.Add(i, new List<Coordinate>());
                for (int j = 0; j < clusters_[i].Count; ++j)
                {
                    other.clusters_[i].Add(clusters_[i][j]);
                }
            }
            other.costs_ = new List<double>();
            foreach (var cost in costs_)
            {
                other.costs_.Add(cost);
            }
            return other;
        }
        public override void MakeNewCenters()
        {
            Random r = new Random();
            double cost = 0;
            for (int i = 0; i < clusters_.Count; ++i)
            {
                Coordinate actual = clusters_[i][r.Next(0, clusters_[i].Count)];
                foreach (var point in clusters_[i])
                {
                    cost += Math.Sqrt(Math.Pow((actual.X_ - point.X_), 2) + Math.Pow((actual.Y_ - point.Y_), 2));
                }
                if(cost < costs_[i])
                {
                    centers_[i] = actual;
                }
                cost = 0;
            }
        }
    }
    public struct Coordinate
    {
        public Coordinate(double x, double y)
        {
            X_ = x;
            Y_ = y;
        }
        public double X_ { get;}
        public double Y_ { get;}
    }


    class Program
    {
        static public List<Coordinate> Initialize(int number, List<Coordinate> Points)
        {
            Random r = new Random();
            for (int i = 0; i < number; ++i)
            {
                Points.Add(new Coordinate((double)r.Next(0, 1000), (double)r.Next(0, 1000)));
            }
            return Points;
        }
        static public List<Coordinate> InitializeKmedoids(int number, List<Coordinate> Objects, List<Coordinate> Points)
        {
            Random r = new Random();
            for (int i = 0; i < number; ++i)
            {
                Points.Add(Objects[r.Next(0, 1000)]);
            }
            return Points;
        }
        static public void func(Base Clustering, List<Coordinate> Objects,  int numberOfIterations)
        {
            Console.WriteLine();
            Console.WriteLine(Clustering.GetType().ToString().Split('.')[1] + " Algorithm");
            Console.WriteLine("Iteration| Improvment in % (Initial Cost/Current Cost)| Total Cost of all Clusters");
            for (int i = 0; i < numberOfIterations; ++i)
            {
                Console.Write(i + 1 + ".");
                var PR = Clustering.DeepCopy();
                
                Clustering.ClearClusters();
                foreach (var obj in Objects)
                {
                    Clustering.AddToCluster(Clustering.ClosestCenter(obj), obj);
                }
                
                Clustering.CalculateCosts();
                
                Clustering.MakeNewCenters();
                
                Clustering.CompareClusters(Clustering, PR);
            }
        }
        static void Main(string[] args)
        {
            int numberOfCenters = 5;
            int numberOfPoints = 1000;

            List<Coordinate> Objects = Initialize(numberOfPoints, new List<Coordinate>());
            List<Coordinate> CentersMeans = Initialize(numberOfCenters, new List<Coordinate>());
            List<Coordinate> CentersMedoids = InitializeKmedoids(numberOfCenters, Objects, new List<Coordinate>());

            Base KM = new Kmeans(CentersMeans);
            Base Med = new Kmedoids(CentersMedoids);
            
            func(Med, Objects, 20);
            func(KM, Objects, 20);

        }
    }
}
