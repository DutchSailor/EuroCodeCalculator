﻿using Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TabeleEC2.Model;
using static TabeleEC2.Model.KofZaProracunPravougaonogPresekaModelEC;

namespace TabeleEC2
{

    public static class KofZaProracunPravougaonogPresekaEC
    {
        
        public static KofZaProracunPravougaonogPresekaModelEC GetLimitKofZaProracunPravougaonogPresekaEC(BetonModelEC beton)
        {
            if (beton.fck <= 35)
            {
                return new KofZaProracunPravougaonogPresekaModelEC() { εc = -3.5, εs1 = 4.278, ζ = 0.813, ξ = 0.45, μRd = 0.252, ω = 0/*, kd = 0*/ };
            }else return new KofZaProracunPravougaonogPresekaModelEC() { εc = -3.5, εs1 = 6.5, ζ = 0.854, ξ = 0.35, μRd = 0.206, ω = 0/*, kd = 0*/ };
        }

        public static List<KofZaProracunPravougaonogPresekaModelEC> GetList()
        {
            List<KofZaProracunPravougaonogPresekaModelEC> result= new List<KofZaProracunPravougaonogPresekaModelEC>();
            for (double i = -0.1; i >= -3.5; i+=-0.1)
            {
                result.Add(new KofZaProracunPravougaonogPresekaModelEC(i, 20));
            }
            for (double i = 0.5; i < 20; i+=0.5)
            {
                result.Add(new KofZaProracunPravougaonogPresekaModelEC(-3.5, i));
            }
            result.Add(new KofZaProracunPravougaonogPresekaModelEC(-3.5, 20));
            return result.OrderBy(n=>n.μRd).ToList();
        }
        /// <summary>
        /// Msd * 100 / (b * Math.Pow(d, 2) * fcd)
        /// </summary>
        /// <param name="Msd">in kN/m</param>
        /// <param name="b">in cm</param>
        /// <param name="d">in cm</param>
        /// <param name="fcd">in kN/cm</param> 
        /// <returns></returns>
        public static double GetμSd(double Msd, double b, double d, double fcd)
        {
            var result= Msd * 100 / (b * Math.Pow(d, 2) * fcd);
            var max = new KofZaProracunPravougaonogPresekaModelEC(-3.5,0.5).μRd;
            var min = new KofZaProracunPravougaonogPresekaModelEC(-0.1, 20).μRd;
            if (result > max)
                return max;
                //throw new Exception("Diletacija u armaturi i betonu prekoracema; \nPovecajte presek!");
            if (result < min)
                throw new Exception("Diletacija u armaturi i betonu prekoracema; \nPovecajte presek!");
            return result;
        }

        public static KofZaProracunPravougaonogPresekaModelEC Get_Kof_From_μ(double μSd,int percision=4)
        {
            var μ_lim = new KofZaProracunPravougaonogPresekaModelEC();
            μ_lim.SetByEcEs1(-3.5, 20);
            KofZaProracunPravougaonogPresekaModelEC kofResult =new KofZaProracunPravougaonogPresekaModelEC();

            if (μSd > μ_lim.μRd)
            {
                var max_εs1 = 20.0;
                var min_εs1 = 0.0;

                var test_εs1 = (max_εs1 + min_εs1) / 2; 
                var adder_εs1 = test_εs1;


                while (μSd.Round(percision) != kofResult.μRd.Round(percision))
                {

                    adder_εs1 = adder_εs1 / 2;

                    kofResult = new KofZaProracunPravougaonogPresekaModelEC(-3.5, test_εs1);
                    if (kofResult.μRd > μSd) test_εs1 += adder_εs1;
                    else test_εs1 -= adder_εs1;
                }
            }
            else if (μSd < μ_lim.μRd)
            {
                var max_εc = 0.0;
                var min_εc = -3.5;

                var test_εc = (max_εc + min_εc) / 2;
                var adder_εc = test_εc;

                while (μSd.Round(percision) != kofResult.μRd.Round(percision))
                {
                    adder_εc = adder_εc / 2;
                    kofResult = new KofZaProracunPravougaonogPresekaModelEC( test_εc, 20);
                    if (kofResult.μRd > μSd) test_εc -= adder_εc;
                    else test_εc += adder_εc;
                }
            }
            else return μ_lim;

            return kofResult;
        } 
    }
    public static class KofZaProracunPravougaonogPresekaPBAB 
    {
        public static List<KofZaProracunPravougaonogPresekaModelPBAB> GetListOfKoficientsPBAB(double percision = 0.05)
        {
            double εc = 0.05; double εs1 = -0.50;
            var result = new List<KofZaProracunPravougaonogPresekaModelPBAB>();

            for (double i = εc; i <= 3.5; i += percision)
            {
                var j = 10;
                var n = new KofZaProracunPravougaonogPresekaModelPBAB(-i, j);
                result.Add(n);
            }
            for (double j = εs1; j <= 10; j += percision)
            {
                var i = -3.5;
                var n = new KofZaProracunPravougaonogPresekaModelPBAB(i, j);
                result.Add(n);
            }
            return result;

        }
        public static KofZaProracunPravougaonogPresekaModelPBAB GetLimitKofZaProracunPravougaonogPresekaPBAB()
        {
            return new KofZaProracunPravougaonogPresekaModelPBAB(-3.5, 6.5);
        }

        private static List<KofZaProracunPravougaonogPresekaModelPBAB> GetListTacna_εb(double max_εa1, double min_εa1, double percision = 0.0001)
        {
            var result = new List<KofZaProracunPravougaonogPresekaModelPBAB>(); 
            var εc = Math.Abs(-3.5);

            for (double j = min_εa1; j <= max_εa1; j += percision)
            {
                var i = 3.5;
                var n = new KofZaProracunPravougaonogPresekaModelPBAB(-i, j);
                result.Add(n);
            }
            return result;
        }
        private static List<KofZaProracunPravougaonogPresekaModelPBAB> GetListTacna_εa1(double max_εb, double min_εb, double percision = 0.0001)
        {
            var result = new List<KofZaProracunPravougaonogPresekaModelPBAB>();

            for (double i = Math.Abs(min_εb); i <= Math.Abs(max_εb); i += percision)
            {
                var j = 10;
                var n = new KofZaProracunPravougaonogPresekaModelPBAB(-i, j);
                result.Add(n);
            }
            return result;
        }
        public static KofZaProracunPravougaonogPresekaModelPBAB Get_Kof_From_k(double k)
        { 
            var k_lim = new KofZaProracunPravougaonogPresekaModelPBAB(-3.5, 10);

            if (k > k_lim.k)
            {
                var kof_iz_tablice = GetItem_Full(k);
                var max_εa1 = kof_iz_tablice.εa1 + 0.5;
                var min_εa1 = kof_iz_tablice.εa1 - 0.5;
                var ListToSerachForKof = new List<KofZaProracunPravougaonogPresekaModelPBAB>();
                ListToSerachForKof.AddRange(GetListTacna_εa1(max_εa1, min_εa1));

                return GetSearchFork(k, ListToSerachForKof);
            }
            if (k < k_lim.k)
            {
                var kof_iz_tablice = GetItem_Full(k: k);
                var max_εb = kof_iz_tablice.εb + 0.5;
                var min_εb = kof_iz_tablice.εb - 0.5;
                var ListToSerachForKof = new List<KofZaProracunPravougaonogPresekaModelPBAB>();
                ListToSerachForKof.AddRange(GetListTacna_εb(max_εb, min_εb));

                return GetSearchFork(k, ListToSerachForKof);
            }
            return k_lim;

        }
        private static KofZaProracunPravougaonogPresekaModelPBAB GetSearchFork(double k, List<KofZaProracunPravougaonogPresekaModelPBAB> listToSearch)
        {
            var list = listToSearch;
            if (listToSearch.Count == 0)
            {
                var nul = new KofZaProracunPravougaonogPresekaModelPBAB(0, 20);
                return nul;
            }

            double closest = list.Select(x => x.k)
                .Select(n => new { n, distance = Math.Abs(n - k) })
                .OrderBy(p => p.distance)
                .First(i => i.n >= k).n;

            return listToSearch.Single(x => x.k == closest);
        }


        public static KofZaProracunPravougaonogPresekaModelPBAB GetItem_Full(double k)
        {
        var list = GetListOfKoficientsPBAB();
            var item = list.Single(n => n.k == GetItem_k(k, list));

            return item;
        }
        private static double GetItem_k(double k, List<KofZaProracunPravougaonogPresekaModelPBAB> listToSearch)
        {
            var list = listToSearch.Select(n => n.k);

            double closest = list
                .Select(n => new { n, distance = Math.Abs(n - k) })
                .OrderBy(p => p.distance)
                .First(i => i.n >= k).n;

            return closest;
        }
    }
}
