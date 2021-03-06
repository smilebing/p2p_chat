﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    public class MyTreaty
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public DateTime Date { get; set; }
        public string FileName{ get; set; }
        public string Pwd { get; set; }
        public SortedList<string, Client_statue> online_clients = new SortedList<string, Client_statue>();


        public MyTreaty(int type, string name,string pwd, byte[] content,DateTime date,string fileName,SortedList<string, Client_statue> clients=null)
        {
            this.Type = type;
            this.Name = name;
            this.Pwd = pwd;
            this.Content = content;
            this.Date = date;
            this.FileName = fileName;
            online_clients = clients;
        }

        //public MyTreaty(int type, string name, byte[] content, DateTime date, string fileName)
        //{
        //    this.Type = type;
        //    this.Name = name;
        
        //    this.Content = content;
        //    this.Date = date;
        //    this.FileName = fileName;
        //}
    
 

        // 序列化
        public byte[] GetBytes()
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();

            brFormatter.Serialize(memStream, this);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }
    


        // 反序列化   
        public static MyTreaty GetMyTreaty(byte[] byPacket)
        {
            MemoryStream memStream = new MemoryStream(byPacket);
            IFormatter brFormatter = new BinaryFormatter();
            MyTreaty myt;
            try
            {
                myt = (MyTreaty)brFormatter.Deserialize(memStream);
            }
            catch (Exception)
            {
                throw new InvalidCastException("序列化对象结构发生变化，请删除缓存文件后再操作");
            }
            memStream.Close();
            memStream.Dispose();
            return myt;
        }
    }
}
