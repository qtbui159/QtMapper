using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qt
{
    public interface IMapper
    {
        /// <summary>
        /// 默认规则绑定，类型和名字相同的属性自动绑定。如果为迭代类型，
        /// 可绑定List,Array转换；超出该范围请自行用自定义规则绑定
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <returns></returns>
        IMapper Bind<TDst, TSrc>();
        /// <summary>
        /// 自定义规则绑定
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        IMapper Bind<TDst, TSrc>(Func<TSrc, TDst> func);
        /// <summary>
        /// 额外绑定，可以将默认规则以外的量进行绑定
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="func"></param>
        void BindExtra<TDst, TSrc>(Func<TDst, TSrc, TDst> func);
        /// <summary>
        /// 取转换结果
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        TDst Get<TDst, TSrc>(TSrc src);
        /// <summary>
        /// 取转换结果
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        TDst Get<TDst>(object obj);
    }
}
