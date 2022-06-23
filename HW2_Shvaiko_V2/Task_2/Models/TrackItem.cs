using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Task_2.Models
{
    public class TrackItem
    {

        public int Id { get; set; }
        public bool IsPlaying { get; set; }
        //полный путь к треку
        public string Path { get; set; }
        public string  Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set;}

        public string Duration { get; set; }

        public string BitRate { get; set; }
        public string Khz { get; set; }
        public TrackItem(string path)
        {
            IsPlaying = false;
            Path = path;
            try
            {
                TagLib.File tagFile = TagLib.File.Create(Path);
                Artist = tagFile.Tag.FirstAlbumArtist == null
                    ? string.Join(",", tagFile.Tag.Performers)
                    : tagFile.Tag.FirstAlbumArtist;
                Album = tagFile.Tag.Album;
                Title = tagFile.Tag.Title;
                Duration = tagFile.Properties.Duration.ToString("mm\\:ss");
                BitRate = tagFile.Properties.AudioBitrate.ToString();
                Khz = (tagFile.Properties.AudioSampleRate / 1000).ToString("F1");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message,
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }//Track

        public TrackItem() { }
       /* public override string ToString() =>$"{Artist} - {Title}                                 {Duration}"  ;*/
        
    }//Track
}//Task_2.Models
