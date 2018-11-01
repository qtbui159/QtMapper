using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qt;
using System.Collections.Generic;

namespace QtMapper.Test
{
    [TestClass]
    public class DefaultBindTest
    {
        private IMapper m_Mapper = new Qt.QtMapper();

        public DefaultBindTest()
        {
            m_Mapper.Bind<DefaultBindDst, DefaultBindSrc>().BindExtra<DefaultBindDst, DefaultBindSrc>((dst, src) =>
            {
                dst.Integer = src.Integer + 1;
                return dst;
            });

            m_Mapper.Bind<List<DefaultBindDst>, List<DefaultBindSrc>>();

            m_Mapper.Bind<DefaultBindDst[], DefaultBindSrc[]>();

            m_Mapper.Bind<List<DefaultBindDst>, DefaultBindSrc[]>();
            m_Mapper.Bind<DefaultBindDst[], List<DefaultBindSrc>>();
        }

        [TestMethod]
        public void TestDefaultBind()
        {
            DefaultBindSrc defaultBindSrc = new DefaultBindSrc()
            {
                Integer = 10086,
                Str = "Hello",
            };
            DefaultBindDst defaultBindDst = m_Mapper.Get<DefaultBindDst>(defaultBindSrc);

            Assert.AreEqual(10086+1, defaultBindDst.Integer);
            Assert.AreEqual("Hello", defaultBindDst.Str);
        }

        [TestMethod]
        public void TestDefaultBind_List()
        {
            List<DefaultBindSrc> defaultBindSrcList = new List<DefaultBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                defaultBindSrcList.Add(new DefaultBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            List<DefaultBindDst> defaultBindDstList = m_Mapper.Get<List<DefaultBindDst>>(defaultBindSrcList);

            Assert.AreEqual(defaultBindSrcList.Count, defaultBindDstList.Count);

            for (int i = 0, count = defaultBindDstList.Count; i < count; ++i)
            {
                Assert.AreEqual(defaultBindSrcList[i].Integer + 1, defaultBindDstList[i].Integer);
                Assert.AreEqual(defaultBindSrcList[i].Str, defaultBindDstList[i].Str);
            }
        }

        [TestMethod]
        public void TestDefaultBind_Array()
        {
            List<DefaultBindSrc> defaultBindSrcList = new List<DefaultBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                defaultBindSrcList.Add(new DefaultBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            DefaultBindSrc[] defaultBindSrcArray = defaultBindSrcList.ToArray();

            DefaultBindDst[] defaultBindDstArray = m_Mapper.Get<DefaultBindDst[]>(defaultBindSrcArray);

            Assert.AreEqual(defaultBindSrcArray.Length, defaultBindDstArray.Length);

            for (int i = 0, count = defaultBindDstArray.Length; i < count; ++i)
            {
                Assert.AreEqual(defaultBindSrcArray[i].Integer + 1, defaultBindDstArray[i].Integer);
                Assert.AreEqual(defaultBindSrcArray[i].Str, defaultBindDstArray[i].Str);
            }
        }

        [TestMethod]
        public void TestDefaultBind_Array_List()
        {
            List<DefaultBindSrc> defaultBindSrcList = new List<DefaultBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                defaultBindSrcList.Add(new DefaultBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            DefaultBindDst[] defaultBindDstArray = m_Mapper.Get<DefaultBindDst[]>(defaultBindSrcList);

            Assert.AreEqual(defaultBindSrcList.Count, defaultBindDstArray.Length);

            for (int i = 0, count = defaultBindDstArray.Length; i < count; ++i)
            {
                Assert.AreEqual(defaultBindSrcList[i].Integer + 1, defaultBindDstArray[i].Integer);
                Assert.AreEqual(defaultBindSrcList[i].Str, defaultBindDstArray[i].Str);
            }
        }

        [TestMethod]
        public void TestDefaultBind_List_Array()
        {
            List<DefaultBindSrc> defaultBindSrcList = new List<DefaultBindSrc>();
            for (int i = 1; i <= 10; ++i)
            {
                defaultBindSrcList.Add(new DefaultBindSrc()
                {
                    Integer = 10086 + i,
                    Str = "Hello" + i.ToString(),
                });
            }

            DefaultBindSrc[] defaultBindSrcArray = defaultBindSrcList.ToArray();

            List<DefaultBindDst> defaultBindDstList = m_Mapper.Get<List<DefaultBindDst>>(defaultBindSrcArray);

            Assert.AreEqual(defaultBindSrcArray.Length, defaultBindDstList.Count);

            for (int i = 0, count = defaultBindDstList.Count; i < count; ++i)
            {
                Assert.AreEqual(defaultBindSrcArray[i].Integer + 1, defaultBindDstList[i].Integer);
                Assert.AreEqual(defaultBindSrcArray[i].Str, defaultBindDstList[i].Str);
            }
        }
    }

    class DefaultBindSrc
    {
        public int Integer { get; set; }
        public string Str { get; set; }
    }

    class DefaultBindDst
    {
        public int Integer { get; set; }
        public string Str { get; set; }
    }
}
