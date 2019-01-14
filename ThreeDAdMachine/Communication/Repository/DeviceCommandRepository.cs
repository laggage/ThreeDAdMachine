using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Communication.Models;

namespace Communication.Repository
{
    public class DeviceCommandRepository
    {
        public DeviceCommandRepository()
        {
            DetectLocalDbFile();
            XDoc = XDocument.Load(LocalDbFilePath);
        }

        /// <summary>
        /// 存储在用户本地的数据文件
        /// </summary>
        private const string LocalDbFilePath = @"./Cache/DeviceCommandsDb.db";

        private XDocument XDoc { get; }
        private XElement XNodeDeviceCommands => XDoc.Element("DeviceCommands");

        /// <summary>
        /// 探测本地数据库文件是否存在,不存在则创建
        /// </summary>
        private void DetectLocalDbFile()
        {
            //检查目录和文件是否存在
            string containerDirectory = Directory.GetParent(LocalDbFilePath).FullName;
            if (!Directory.Exists(containerDirectory)) Directory.CreateDirectory(containerDirectory);
            if (!File.Exists(LocalDbFilePath)) File.Create(LocalDbFilePath);
            //检查文件的内容是否符合要求,不符合则在文件中建立新的xml文档,并建立Persons节点作为根节点
            XDocument xd = null;
            try
            {
                xd = XDocument.Load(LocalDbFilePath);
                XElement xe = xd.Element("DeviceCommands");
            }
            catch (Exception)
            {
                xd = new XDocument(new XElement("DeviceCommands"));
            }
            finally
            {
                xd?.Save(LocalDbFilePath);
            }
        }

        private XElement GenerateDeviceCommandNode(string commandName, int commandCode)
        {
            return new XElement("DeviceCommand", new XAttribute("Name", commandName),
                new XAttribute("Code", commandCode));
        }

        public bool Add(string commandName, int commandCode)
        {
            try
            {
                if (XNodeDeviceCommands.Descendants("DeviceCommand").Any((e) => e.Attribute("Name")?.Value == commandName))
                    return false; 
                XNodeDeviceCommands.Add(GenerateDeviceCommandNode(commandName, commandCode));
                XDoc.Save(LocalDbFilePath);
                return true;
            }
            catch (Exception) { return false; }

        }

        public ObservableCollection<DeviceCommand> Load()
        {
            ObservableCollection<DeviceCommand> commands = new ObservableCollection<DeviceCommand>();
            foreach (XElement item in XDoc.Descendants("DeviceCommand"))
            {
                commands.Add(new DeviceCommand(item.Attribute("Name")?.Value, 
                    int.Parse(item.Attribute("Code")?.Value))
                );
            }
            return commands;
        }

        public bool Remove(DeviceCommand deviceCommand)
        {
            return Remove(deviceCommand.CommandCode);
        }

        public bool Remove(int commandCode)
        {
            try
            {
                XNodeDeviceCommands
                    .Descendants("DeviceCommand")
                    .Single(element => int.Parse(element.Attribute("Code").Value) == commandCode).Remove();
                return true;
            }
            catch(Exception) { return false; }
        }
    }
}
