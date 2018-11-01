using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QtMapper.Test
{
    [TestClass]
    public class CustomBindTest
    {
        private IMapper m_Mapper = new Qt.QtMapper();

        public CustomBindTest()
        {
            m_Mapper.Bind<CustomBindDst, CustomBindSrc>(src =>
            {
                CustomBindDst dst = new CustomBindDst();
                dst.Integer = src.Integer + 12580;
                dst.Str = src.Str + "world";
                return dst;
            });

            m_Mapper.Bind<List<CustomBindDst>, List<CustomBindSrc>>(src =>
            {
                List<CustomBindDst> dst = new List<CustomBindDst>();
                for (int i = 0, count = src.Count; i < count; ++i)
                {
                    CustomBindDst tmp = new CustomBindDst();
                    tmp.Integer = src[i].Integer + 11111;
                    tmp.Str = src[i].Str + "Left4Dead2";
                    dst.Add(tmp);
                }
                return dst;
            });

            m_Mapper.Bind<CustomBindDst[], CustomBindSrc[]>(src =>
            {
                CustomBindDst[] dst = new CustomBindDst[src.Length];
                for (int i = 0, count = src.Length; i < count; ++i)
                {
                    CustomBindDst tmp = m_Mapper.Get<CustomBindDst>(src[i]);
                    dst[i]=(tmp);
                }
                return dst;
            });


            m_Mapper.Bind<List<CustomBindDst>, CustomBindSrc[]>(src =>
            {
                CustomBindDst[] dst = new CustomBindDst[src.Length];
                for (int i = 0, count = src.Length; i < count; ++i)
                {
                    CustomBindDst tmp = m_Mapper.Get<CustomBindDst>(src[i]);
                    dst[i] = (tmp);
                }
                return dst.ToList();
            });

            m_Mapper.Bind<CustomBindDst[], List<CustomBindSrc>>(src =>
            {
                CustomBindDst[] dst = new CustomBindDst[src.Count];
                for (int i = 0, count = src.Count; i < count; ++i)
                {
                    CustomBindDst tmp = m_Mapper.Get<CustomBindDst>(src[i]);
                    dst[i] = (tmp);
                }
                return dst;
            });
        }

        [TestMethod]
        public void TestCustomeBind()
        {
            CustomBindSrc defaultBindSrc = new CustomBindSrc()
            {
                Integer = 10086,
                Str = "Hello",
            };
            CustomBindDst defaultBindDst = m_Mapper.Get<CustomBindDst>(defaultBindSrc);

            Assert.AreEqual(10086 + 12580, defaultBindDst.Integer);
            Assert.AreEqual("Hello" + "world", defaultBindDst.Str);
        }

        [TestMethod]
        public void TestCustomeBind_List()
        {
            List<CustomBindSrc> customBindSrcList = new List<CustomBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                customBindSrcList.Add(new CustomBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            List<CustomBindDst> customBindDstList = m_Mapper.Get<List<CustomBindDst>>(customBindSrcList);

            Assert.AreEqual(customBindSrcList.Count, customBindDstList.Count);

            for (int i = 0, count = customBindDstList.Count; i < count; ++i)
            {
                Assert.AreEqual(customBindSrcList[i].Integer + 11111, customBindDstList[i].Integer);
                Assert.AreEqual(customBindSrcList[i].Str + "Left4Dead2", customBindDstList[i].Str);
            }
        }

        [TestMethod]
        public void TestCustomeBind_Array()
        {
            List<CustomBindSrc> customBindSrcList = new List<CustomBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                customBindSrcList.Add(new CustomBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            CustomBindDst[] customBindDstArray = m_Mapper.Get<CustomBindDst[]>(customBindSrcList.ToArray());

            Assert.AreEqual(customBindSrcList.Count, customBindDstArray.Length);

            for (int i = 0, count = customBindDstArray.Length; i < count; ++i)
            {
                Assert.AreEqual(customBindSrcList[i].Integer + 12580, customBindDstArray[i].Integer);
                Assert.AreEqual(customBindSrcList[i].Str + "world", customBindDstArray[i].Str);
            }
        }

        [TestMethod]
        public void TestCustomBind_Array_List()
        {
            List<CustomBindSrc> customBindSrcList = new List<CustomBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                customBindSrcList.Add(new CustomBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            CustomBindDst[] customBindDstArray = m_Mapper.Get<CustomBindDst[]>(customBindSrcList);

            Assert.AreEqual(customBindSrcList.Count, customBindDstArray.Length);

            for (int i = 0, count = customBindDstArray.Length; i < count; ++i)
            {
                Assert.AreEqual(customBindSrcList[i].Integer + 12580, customBindDstArray[i].Integer);
                Assert.AreEqual(customBindSrcList[i].Str + "world", customBindDstArray[i].Str);
            }
        }

        [TestMethod]
        public void TestCustomBind_List_Array()
        {
            List<CustomBindSrc> customBindSrcList = new List<CustomBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                customBindSrcList.Add(new CustomBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            CustomBindSrc[] customBindSrcArray = customBindSrcList.ToArray();

            List<CustomBindDst> defaultBindDstList = m_Mapper.Get<List<CustomBindDst>>(customBindSrcArray);

            Assert.AreEqual(customBindSrcArray.Length, defaultBindDstList.Count);

            for (int i = 0, count = defaultBindDstList.Count; i < count; ++i)
            {
                Assert.AreEqual(customBindSrcArray[i].Integer + 12580, defaultBindDstList[i].Integer);
                Assert.AreEqual(customBindSrcArray[i].Str + "world", defaultBindDstList[i].Str);
            }
        }
    }

    class CustomBindSrc
    {
        public int Integer { get; set; }
        public string Str { get; set; }
    }

    class CustomBindDst
    {
        public int Integer { get; set; }
        public string Str { get; set; }
    }
}
