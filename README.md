# QtMapper
A simple Mapper use easily

You can use two methods to convert your objectSrcType to objectDstType easily.

A. Use the Bind(default strategy) which will map the properties or field automatically( same type and name).It works well with BindExtra.

B. Use the Bind(custom strategy) to decide the map rules.
Array and List is also supported. View the test code you can get more help :)

Get this library [nuget][1] or build the project.

Code sample:

    IMapper mapper=new QtMapper();
    mapper.Bind<Dst,Src>()   //<-- now you bind members which the same type and name
          .BindExtra<Dst,Src>((dst,src)=>
          {
              dst.NewMember=src.OldMember;
              return dst;
          });                //<--- now you bind the other members        
    
or like this
    
    mapper.Bind<Dst,Src>(src=>
    {
        Dst dst=new Dst();
        dst.Member1=src.Member1;
        dst.Member2=src.Member2;
        dst.NewMember=src.OldMember;
        return dst;
    });
    
Now you can use it:

    Src src=new Src();
    
    mapper.Get<Dst>(src);

If you want the convert Array or List:

    mapper.Bind<List<Dst>,List<Src>>(); //<--- but you should Bind<Dst,Src> firstly
    
    mapper.Get<List<Dst>>(srcList);
    
If you want the complex type convert:
    
    mapper.Bind<Dictionary<Dst,int>,Src>(src=>
    {
        Dictionary<Dst,int> dst=new Dictionary<Dst,int>();
        //do bind
        return dst;
    });
    
    
    
转换方法有两种，
第一种，默认规则转换(Bind)，根据类型和字段名，配合额外转换(BindExtra)使用；
第二种，自定义规则转换(Bind)，直接写下自定义规则即可；
支持Array和List的绑定，其他复杂类型需要全部写自定义规则，详情参阅测试代码和例子代码。
    
  [1]: https://www.nuget.org/packages/QtMapper/0.2.0