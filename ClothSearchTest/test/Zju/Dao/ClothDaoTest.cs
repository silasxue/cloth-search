﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Zju.Util;
using Zju.Domain;

namespace Zju.Dao
{
    [TestFixture]
    public class ClothDaoTest
    {
        private ClothDao target;
        private Cloth cloth;
        private Cloth cloth2;
        private List<Cloth> onlyList;
        private List<Cloth> emptyList;
        private List<Cloth> clothes;

        [TestFixtureSetUp]
        public void SetUp()
        {
            target = new ClothDao();

            cloth = new Cloth("n", "p", "p", ColorEnum.BLACK | ColorEnum.BLUE, ShapeEnum.CIRCLE | ShapeEnum.SQUARE);
            cloth2 = new Cloth("nn", "pp", "pp", ColorEnum.RED | ColorEnum.DARKRED, ShapeEnum.NONE);
            onlyList = new List<Cloth>();
            onlyList.Add(cloth);

            emptyList = new List<Cloth>();

            clothes = new List<Cloth>();
            for (int i=0; i<100; ++i)
            {
                ColorEnum color = (ColorEnum)(2 << (i % 6) >> 1);
                ShapeEnum shape = (ShapeEnum)(2 << (i % 5) >> 1);
                Cloth c = new Cloth("n" + i, "p" + i, "p" + i, color, shape);
                clothes.Add(c);
            }
        }

        private void print(Cloth cloth)
        {
            Console.WriteLine(cloth.Name + "," + cloth.UpdateTime + "," + cloth.Oid + "," + cloth.Colors);
        }

        [Test]
        public void TestBasicCRUD()
        {
            // save
            //print(cloth);
            target.SaveOrUpdate(cloth);
            //print(cloth);
            Assert.AreEqual(cloth, target.FindByOid(cloth.Oid));
            Assert.AreEqual(onlyList, target.FindAll());
            Assert.AreEqual(onlyList, target.FindAllByColors(ColorEnum.BLACK));
            Assert.AreEqual(onlyList, target.FindAllByColors(ColorEnum.BLUE));
            Assert.AreEqual(onlyList, target.FindAllByColors(ColorEnum.BLACK | ColorEnum.BLUE));
            Assert.AreEqual(onlyList, target.FindAllByColors(ColorEnum.BLACK, ColorEnum.PINK));
            Assert.AreEqual(onlyList, target.FindAllByColors(ColorEnum.NONE));
            Assert.AreEqual(onlyList, target.FindAllByShapes(ShapeEnum.CIRCLE));
            Assert.AreEqual(onlyList, target.FindAllByShapes(ShapeEnum.NONE));
            // the result should be empty list
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.PINK));
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.BLACK, ColorEnum.BLUE));
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.BLACK | ColorEnum.PINK));
            
            // update
            int oid = cloth.Oid;
            cloth.Path = "abcd";
            cloth.Name = "na";
            cloth.UpdateTime = DateTime.UtcNow;
            cloth.Colors = ColorEnum.WHITE | ColorEnum.PINK;
            //print(cloth);
            target.SaveOrUpdate(cloth);
            //print(cloth);
            Assert.AreEqual(cloth, target.FindByOid(oid));
            Assert.AreEqual(onlyList, target.FindAll());
            Assert.AreEqual(onlyList, target.FindAllByColors(ColorEnum.WHITE));
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.BLUE));
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.BLACK | ColorEnum.BLUE));
            //print(cloth);
            // delete the cloth, for next test case
            target.Delete(oid);
            //print(cloth);
            Assert.IsNull(target.FindByOid(oid));
            Assert.AreEqual(emptyList, target.FindAll());
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.WHITE));
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.BLUE));
            Assert.AreEqual(emptyList, target.FindAllByColors(ColorEnum.BLACK | ColorEnum.BLUE));
        }

        [Test]
        public void TestSaveOrUpdateAll()
        {
            target.SaveOrUpdateAll(clothes);
            Assert.AreEqual(clothes, target.FindAll());
            Assert.AreEqual((clothes.Count + 5) / 6, target.FindAllByColors(ColorEnum.BLACK).Count);
            foreach (Cloth cloth in clothes)
            {
                target.Delete(cloth.Oid);
            }
            Assert.AreEqual(emptyList, target.FindAll());
            Assert.AreEqual(0, target.FindAllByColors(ColorEnum.BLACK).Count);
        }
/*
        [Test]
        public void TestDelete()
        {

        }

        [Test]
        public void TestFindAllByColors()
        {

        }

        [Test]
        public void TestFindAllByShapes()
        {

        }
*/
        

        [TestFixtureTearDown]
        public void TearDown()
        {
            DaoHelper.CloseDb();
        }
    }
}