﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabeleEC2;
using TabeleEC2.Model;

namespace TabeleEC2.Test
{
    [TestClass]
    public class BetonModelEC2Test
    {
        [TestMethod]
        [DataRow("C20/25",20,25)]
        [DataRow("C25/30",25,30)]
        public void BetonModelEC2PassClassReturnFck(string name, int exp1, int exp2)
        {
            var beton = new BetonModelEC_v2()
            {
                name = name,
            };

            Assert.AreEqual(exp1, beton.fck);
            Assert.AreEqual(exp2, beton.fck_cube);
            Assert.AreEqual(exp1+8, beton.fcm); 
            Assert.AreEqual(0.30 * Math.Pow(exp1, 2 / 3), beton.fctm);
            Assert.AreEqual(0.70 * beton.fctm, beton.fctk005);
            Assert.AreEqual(1.30 * beton.fctm, beton.fctk095);
            Assert.AreEqual(0.85*beton.fck/1.5, beton.fcd);
        }

        [TestMethod]
        [DataRow(null)]
        public void BetonModelEC2PassNullClassNameReturnExpetatin(string name)
        {
            Assert.ThrowsException<NullReferenceException>(()=> new BetonModelEC_v2()
            {
                name = name,
            }.fck
            );
        }
    }
}
