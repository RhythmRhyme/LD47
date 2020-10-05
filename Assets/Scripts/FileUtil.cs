
using System;
using System.IO;
using System.Text;

public static class FileUtil
{
    /*
    * path：文件创建目录
    * name：文件的名称
    *  info：写入的内容
    */
    public static void CreateFile(string path, string name, string info)
    {
        //文件流信息
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);

        //如果此文件不存在则创建
        sw = t.CreateText();

        //以行的形式写入信息
        sw.WriteLine(info);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }    
    
    /*
     * path：读取文件的路径
     * name：读取文件的名称
     */
    public static string LoadFile(string path, string name)
    {
        //使用流的形式读取
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e)
        {
            //路径与名称未找到文件则直接返回空
            return null;
        }
        string arrlist;
        arrlist = sr.ReadLine();
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回
        return arrlist;
    }
    
    public static string Base64Encode(string str)  
    {  
        string go = Convert.ToBase64String(Encoding.Default.GetBytes(str));
        return Convert.ToBase64String(Encoding.Default.GetBytes(go));
    } 
}
