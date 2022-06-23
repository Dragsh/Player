using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Task_2.Models
{
    public class PlayList
    {
        public ObservableCollection<TrackItem> Tracks { get; set; }

        public PlayList()
        {
            Tracks = new ObservableCollection<TrackItem>();
        }//PlayList()

        

        //сохранить данные в xml файл
        public void SaveXml(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<TrackItem>));

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
                formatter.Serialize(fs, Tracks);
        } // SaveXML

        //загрузить данные
        public void LoadXml(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<TrackItem>));
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                Tracks = (ObservableCollection<TrackItem>)formatter.Deserialize(fs);
        } // LoadXML


    }//PlayList
}//Task_2.Models
