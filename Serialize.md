# 序列化 #
什么是类的序列化？说白了，就是把一个类的实例转化成一段XML格式或二进制格式的数据，以便于网络传输、保存等操作。
同理，反序列化就是把XML或者二进制描述的对象还原成一个类的实例。

# 零、开始序列化 #
在C#中，要实现类的序列化并不难，以XML序列化为例，首先我们声明一个类：
```
[Serializable]
public class myClass
{
......
}
```
其中类声明上面的一句[Serializable](Serializable.md)用来指示此类是可以序列化的，然后
引用两个namespace：
```
using System.IO;
using System.Xml.Serialization;
```
于是就可以执行下面代码：
```
myClass cls = new myClass();
XmlSerializer xmlSerializer = new XmlSerializer(cls.GetType());
MemoryStream stream = new MemoryStream();
xmlSerializer.Serialize(stream, cls);
byte[] buf = stream.ToArray();
string xml = Encoding.ASCII.GetString(buf);
stream.Close();
```
自此，序列化就完成了，XML序列保存在string xml;变量中。

上述代码不难理解，XmlSerializer类用来提供XML序列化的功能，xmlSerializer.Serialize(stream, cls)方法，可以把类cls序列化，并将XML序列存储在流stream中。
以上便是序列化的基本方法，可以满足我们的需求。但问题是如果我们经常需要对数个类进行序列化和反序列化，就要频繁的重复上述代码，能不能让类具备自己进行序列化的方法呢？

# 一、第一次尝试 #
我首先想到，可以写一个基类，提供进行序列化的方法，任何想实现序列化的类，只需要继承此类就可以具备此方法。于是构造抽象类：
```
public abstract class SerializableBaseClass
{
public virtual string XMLSerialize()
{
XmlSerializer xmlSerializer = new XmlSerializer(GetType()); //差异1
MemoryStream stream = new MemoryStream();
xmlSerializer.Serialize(stream, this); //差异2
byte[] buf = stream.ToArray();
string xml = Encoding.ASCII.GetString(buf);
stream.Close();
return xml;
}
}
```
上面一段代码和之前的代码只有两行不一样：
## 差异1： ##
cls.GetType()变成GetType()：其中GetType()是获取当前实例的类型，在基类中调用GetType()得到的是当前实例的类型，而不是积累的类型。也就是说上述基类中调用GetType()不会得到” SerializableBaseClass”，更不会是”System.Object”，而是当前对象实例的类型。

## 差异2： ##
cls变成this：同理，this引用是指向当前实例的，只不过在基类中，不能使用this直接访问子类成员。当然通过类型转换可以达到此目的，但不在本文讨论范围内。所以Serialize(stream,this)会将整个对象序列化，而不会造成对象分割，只把基类给序列化了。

# 二、第二次尝试 #
到此为止，任何类只要集成了SerializableBaseClass就拥有了自我序列化的方法，但如何反序列化呢？我们在SerializableBaseClass类中再添加一个方法。
```
public object DeSerialize(string xmlString)
{
XmlSerializer xmlSerializer = new XmlSerializer(GetType());
byte[] buf = Encoding.ASCII.GetBytes(xmlString);
MemoryStream stream = new MemoryStream(buf);
object o = xmlSerializer.Deserialize(stream);
return o;
}
```
此方法就实现了类的反序列化，我们可以这样使用：

声明：
```
[Serializable]
public class myClass : SerializableBaseClass
{
……
}
```
使用：
```
myClass cls = new myClass();
string xml = cls. Serialize();
myClass cls1 = (myClass)cls. DeSerialize(xml);
```
这个使用乍一看没什么问题，但实际使用起来就很蹩脚
  * 1、要想反序列化一个类，先要创建这个类的实例，只为了调用DeSerialize()
  * 2、调用DeSerialize()返回的是object类型，需要进行类型转换

对于这两个问题，我也考虑过很久，如果把DeSerialize()定义为static，那么就不能使用GetType()方法，而其，也无法获取子类的类型。

# 三、最后的尝试 #
对于上述两个问题，想了很久，始终没想到解决办法，直到一天和朋友讨论c++STL的某个问题的时候，终于茅塞顿开，c#也是支持模板的呀，于是激动不已，改写SerializableBaseClass类：
```
[Serializable()]
public abstract class SerializableBaseClass
{
public virtual string XMLSerialize()
{
XmlSerializer xmlSerializer = new XmlSerializer(GetType());
MemoryStream stream = new MemoryStream();
xmlSerializer.Serialize(stream, this);
byte[] buf = stream.ToArray();
string xml = Encoding.ASCII.GetString(buf);
stream.Close();
return xml;
}
```
这样，问题就不完美的解决了（确实不完美）。为什么不完美呢？那就看下面的代码：
```
[Serializable]
public class myClass : SerializableBaseClass
{
……
}
```
使用：
```
myClass cls = myClass.DeSerialize(xmlData);
```
看似上述问题都解决了，但我还是心里不爽，因为每次定义一个继承SerializableBaseClass的类，还必须把自己的类名再写一遍，放在模板类型的参数里。有没有一种方案，可以让上述myClass类的声明默认模板参数就是自身myClass，而无须再写一遍呢？或者还有更好的其他方法？欢迎讨论。
```
public static T DeSerialize(string xmlString)
{
XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
byte[] buf = Encoding.ASCII.GetBytes(xmlString);
MemoryStream stream = new MemoryStream(buf);
T o = (T)xmlSerializer.Deserialize(stream);
return o;
}
```