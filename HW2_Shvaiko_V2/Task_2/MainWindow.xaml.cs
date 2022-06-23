using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Task_2.Models;

namespace Task_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Доделать:
        //переделать клики на команды
        //Оптимизировать код
        //Убрать ноль, как порядковый номер,  для  первой песни и ничего не сломать
        //Сделать эквалайзер
        //добавить контексное меню в плейлист
        //Добавить окно информации о выбранном треке в контексное меню плейлиста
        //сделать темы оформления

        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;

        // для сохранения и загрузки треков при старте программы(список песен оставшийся до закрытия программы, загружается при старте)
        private string _saveList = @"..\..\playListSave.xml";

        private PlayList _playList;
        
        //Треки выбираются двойным кликом мышки

        public MainWindow()
        {
            InitializeComponent();
            _playList = new PlayList();
            DispatcherTimer timer = new DispatcherTimer();
            LbxTreks.ItemsSource = _playList.Tracks;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            //Запуск таймера для движения ползунка слайдера
            timer.Start();
            stsMainUpdate();

        }//InitializeComponent();

        

        #region методы слайдбара

        //связь слайдера с таймером
        private void timer_Tick(object sender, EventArgs e)
        {
            if (MediaElementPlayer.Source != null && MediaElementPlayer.NaturalDuration.HasTimeSpan && !userIsDraggingSlider)
            {
                SbProgress.Minimum = 0;
                SbProgress.Maximum = MediaElementPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SbProgress.Value = MediaElementPlayer.Position.TotalSeconds;
                LblStatusTrek.Content = string.Format("{0} / {1}", MediaElementPlayer.Position.ToString(@"mm\:ss"),
                    MediaElementPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }

            else
                LblStatusTrek.Content = "No file selected...";


        }//timer_Tick

        //Проверить, если юзер начинает перетаскивать слайдер воспроизведения
        private void SliderProgres_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }//SliderProgress_DragStarted

        //завершение перемещения 
        private void SliderProgres_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            MediaElementPlayer.Position = TimeSpan.FromSeconds(SbProgress.Value);
        }//sliProgress_DragCompleted

        private void SliderProgres_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxbTimeTrekStatus.Text = TimeSpan.FromSeconds(SbProgress.Value).ToString(@"mm\:ss");
        }//SliderProgres_ValueChanged


        #endregion

        #region События  управления проигрывателя

        private void Play_Trek(object sender, RoutedEventArgs e)
        {
            if (MediaElementPlayer.Source == null) return;
            mediaPlayerIsPlaying = true;
            MediaElementPlayer.Play();

            // создать новый поток исполнения, запустить этот поток
            Thread thread = new Thread(ProgressUpdater);
            thread.IsBackground = true;
            thread.Start();

        }//Play_Trek

        private void Stop_Trek(object sender, RoutedEventArgs e)
        {
            MediaElementPlayer.Stop();
            mediaPlayerIsPlaying = false;

        }//Stop_Trek

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerIsPlaying = false;
            MediaElementPlayer.Pause();
            if (MediaElementPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressPosition.Value = MediaElementPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressPosition.Maximum = MediaElementPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }


        }//Pause_Click

        private void Next_Track(object sender, RoutedEventArgs e)
        {
            switch (mediaPlayerIsPlaying)
            {
                case true:
                    {
                        if (MediaElementPlayer.Source == null) return;
                        if (_playList.Tracks.Count == 0) return;
                        MediaElementPlayer.Stop();
                        mediaPlayerIsPlaying = false;
                        TrackItem track;
                        List<TrackItem> tracks;
                        //получаем путь предыдущего трека
                        try
                        {
                            track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                            //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                            tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                            //Находим какой из треков проигрывается
                            track = tracks.Find(x => x.IsPlaying);
                            //выключаем трек в плейлисте
                            _playList.Tracks[track.Id].IsPlaying = false;

                            switch (CheckBoxShaffl.IsChecked)
                            {
                                case true:
                                    {
                                        int rand = Utils.GetRand(0, _playList.Tracks.Count - 1);
                                        TxbTrekInfo.Text = $"{_playList.Tracks[rand].Artist}-{_playList.Tracks[rand].Title}({_playList.Tracks[rand].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[rand].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[rand].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[rand].Khz}";
                                        _playList.Tracks[rand].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;

                                    }// case true
                                    break;

                                case false:
                                    {
                                        //если проигрываемый трек не является последни в списке, переходим на следующий трек
                                        if (track.Id != _playList.Tracks.Count - 1)
                                        {

                                            TxbTrekInfo.Text = $"{_playList.Tracks[track.Id + 1].Artist}-{_playList.Tracks[track.Id + 1].Title}({_playList.Tracks[track.Id + 1].Duration})";
                                            MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id + 1].Path);
                                            TextBlockBitRate.Text = $"{_playList.Tracks[track.Id + 1].BitRate}";
                                            TextBlock_kHz.Text = $"{_playList.Tracks[track.Id + 1].Khz}";
                                            _playList.Tracks[track.Id + 1].IsPlaying = true;
                                            LbxTreks.ItemsSource = null;
                                            LbxTreks.ItemsSource = _playList.Tracks;
                                            MediaElementPlayer.Play();
                                            mediaPlayerIsPlaying = true;
                                        }//if
                                        else
                                        {
                                            //выключаем последний трек в плейлисте
                                            _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = false;
                                            //запускаем первый трек
                                            TxbTrekInfo.Text = $"{_playList.Tracks[0].Artist}-{_playList.Tracks[0].Title}({_playList.Tracks[0].Duration})";
                                            MediaElementPlayer.Source = new Uri(_playList.Tracks[0].Path);
                                            TextBlockBitRate.Text = $"{_playList.Tracks[0].BitRate}";
                                            TextBlock_kHz.Text = $"{_playList.Tracks[0].Khz}";
                                            _playList.Tracks[0].IsPlaying = true;
                                            LbxTreks.ItemsSource = null;
                                            LbxTreks.ItemsSource = _playList.Tracks;
                                            MediaElementPlayer.Play();
                                            mediaPlayerIsPlaying = true;

                                        }//else

                                    }//case false
                                    break;

                                default:
                                    break;
                            }//switch

                        }//try
                        catch(Exception exception)
                        {
                            MessageBox.Show(exception.Message,
                                "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }//catch

                        
                    }//case true
                    break;

                case false:
                    {
                        if (MediaElementPlayer.Source == null) return;
                        if (_playList.Tracks.Count == 0) return;
                        MediaElementPlayer.Stop();
                        mediaPlayerIsPlaying = false;
                        TrackItem track;
                        List<TrackItem> tracks;
                        //получаем путь предыдущего трека
                        track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                        //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                        tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                        //Находим какой из треков проигрывается
                        track = tracks.Find(x => x.IsPlaying);
                        //выключаем трек в плейлисте
                        _playList.Tracks[track.Id].IsPlaying = false;

                        switch (CheckBoxShaffl.IsChecked)
                        {
                            case true:
                                {
                                    int rand = Utils.GetRand(0, _playList.Tracks.Count - 1);
                                    TxbTrekInfo.Text = $"{_playList.Tracks[rand].Artist}-{_playList.Tracks[rand].Title}({_playList.Tracks[rand].Duration})";
                                    MediaElementPlayer.Source = new Uri(_playList.Tracks[rand].Path);
                                    TextBlockBitRate.Text = $"{_playList.Tracks[rand].BitRate}";
                                    TextBlock_kHz.Text = $"{_playList.Tracks[rand].Khz}";
                                    _playList.Tracks[rand].IsPlaying = true;
                                    LbxTreks.ItemsSource = null;
                                    LbxTreks.ItemsSource = _playList.Tracks;
                                    // MediaElementPlayer.Play();
                                    // mediaPlayerIsPlaying = true;

                                }//case true:
                                break;

                            case false:
                                {
                                    //если проигрываемый трек не является последни в списке, переходим на следующий трек
                                    if (track.Id != _playList.Tracks.Count - 1)
                                    {

                                        TxbTrekInfo.Text = $"{_playList.Tracks[track.Id + 1].Artist}-{_playList.Tracks[track.Id + 1].Title}({_playList.Tracks[track.Id + 1].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id + 1].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[track.Id + 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[track.Id + 1].Khz}";
                                        _playList.Tracks[track.Id + 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        //MediaElementPlayer.Play();
                                        // mediaPlayerIsPlaying = true;
                                    }//if
                                    else
                                    {
                                        //выключаем последний трек в плейлисте
                                        _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = false;
                                        //запускаем первый трек
                                        TxbTrekInfo.Text = $"{_playList.Tracks[0].Artist}-{_playList.Tracks[0].Title}({_playList.Tracks[0].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[0].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[0].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[0].Khz}";
                                        _playList.Tracks[0].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        // MediaElementPlayer.Play();
                                        // mediaPlayerIsPlaying = true;

                                    }//else


                                }//case false:
                                break;

                            default:
                                break;
                        }//switch

                    }//case false
                    break;

            }//switch

        }//Next_Track

        private void Previos_Track(object sender, RoutedEventArgs e)
        {
            switch (mediaPlayerIsPlaying)
            {
                case true:
                    {
                        if (MediaElementPlayer.Source == null) return;
                        if (_playList.Tracks.Count == 0) return;
                        MediaElementPlayer.Stop();
                        mediaPlayerIsPlaying = false;

                        TrackItem track;
                        List<TrackItem> tracks;
                        //получаем путь текущего трека
                        track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                        //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                        tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                        //Находим какой из треков проигрывается
                        track = tracks.Find(x => x.IsPlaying);
                        //выключаем трек в плейлисте
                        _playList.Tracks[track.Id].IsPlaying = false;

                        switch (CheckBoxShaffl.IsChecked)
                        {
                            case true:
                                {
                                    int rand = Utils.GetRand(0, _playList.Tracks.Count - 1);
                                    TxbTrekInfo.Text = $"{_playList.Tracks[rand].Artist}-{_playList.Tracks[rand].Title}({_playList.Tracks[rand].Duration})";
                                    MediaElementPlayer.Source = new Uri(_playList.Tracks[rand].Path);
                                    TextBlockBitRate.Text = $"{_playList.Tracks[rand].BitRate}";
                                    TextBlock_kHz.Text = $"{_playList.Tracks[rand].Khz}";
                                    _playList.Tracks[rand].IsPlaying = true;
                                    LbxTreks.ItemsSource = null;
                                    LbxTreks.ItemsSource = _playList.Tracks;
                                    MediaElementPlayer.Play();
                                    mediaPlayerIsPlaying = true;

                                }//case true
                                break;
                            case false:
                                {
                                    //если проигрываемый трек не является первым в списке, переходим на предыдущий трек трек
                                    if (track.Id != 0)
                                    {
                                        TxbTrekInfo.Text = $"{_playList.Tracks[track.Id - 1].Artist}-{_playList.Tracks[track.Id - 1].Title}({_playList.Tracks[track.Id - 1].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id - 1].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[track.Id - 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[track.Id - 1].Khz}";
                                        _playList.Tracks[track.Id - 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;
                                    }//if
                                    else
                                    {
                                        //выключаем первый трек в плейлисте
                                        _playList.Tracks[0].IsPlaying = false;
                                        //запускаем последний трек
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[_playList.Tracks.Count - 1].Path);
                                        TxbTrekInfo.Text =
                                            $"{_playList.Tracks[_playList.Tracks.Count - 1].Artist}-{_playList.Tracks[_playList.Tracks.Count - 1].Title}({_playList.Tracks[_playList.Tracks.Count - 1].Duration})";
                                        TextBlockBitRate.Text = $"{_playList.Tracks[_playList.Tracks.Count - 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[_playList.Tracks.Count - 1].Khz}";
                                        _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;
                                    }//else

                                }//case false
                                break;

                            default:
                                break;
                        }//switch



                    }//case true:
                    break;

                case false:
                    {
                        if (MediaElementPlayer.Source == null) return;
                        if (_playList.Tracks.Count == 0) return;
                        MediaElementPlayer.Stop();
                        mediaPlayerIsPlaying = false;

                        TrackItem track;
                        List<TrackItem> tracks;
                        //получаем путь текущего трека
                        track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                        //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                        tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                        //Находим какой из треков проигрывается
                        track = tracks.Find(x => x.IsPlaying);
                        //выключаем трек в плейлисте
                        _playList.Tracks[track.Id].IsPlaying = false;

                        switch (CheckBoxShaffl.IsChecked)
                        {
                            case true:
                                {
                                    int rand = Utils.GetRand(0, _playList.Tracks.Count - 1);
                                    TxbTrekInfo.Text = $"{_playList.Tracks[rand].Artist}-{_playList.Tracks[rand].Title}({_playList.Tracks[rand].Duration})";
                                    MediaElementPlayer.Source = new Uri(_playList.Tracks[rand].Path);
                                    TextBlockBitRate.Text = $"{_playList.Tracks[rand].BitRate}";
                                    TextBlock_kHz.Text = $"{_playList.Tracks[rand].Khz}";
                                    _playList.Tracks[rand].IsPlaying = true;
                                    LbxTreks.ItemsSource = null;
                                    LbxTreks.ItemsSource = _playList.Tracks;
                                    MediaElementPlayer.Play();
                                    mediaPlayerIsPlaying = true;

                                }//case true
                                break;

                            case false:
                                {
                                    //если проигрываемый трек не является первым в списке, переходим на предыдущий трек трек
                                    if (track.Id != 0)
                                    {
                                        TxbTrekInfo.Text = $"{_playList.Tracks[track.Id - 1].Artist}-{_playList.Tracks[track.Id - 1].Title}({_playList.Tracks[track.Id - 1].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id - 1].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[track.Id - 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[track.Id - 1].Khz}";
                                        _playList.Tracks[track.Id - 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        // MediaElementPlayer.Play();
                                        // mediaPlayerIsPlaying = true;
                                    }//if
                                    else
                                    {
                                        //выключаем первый трек в плейлисте
                                        _playList.Tracks[0].IsPlaying = false;
                                        //запускаем последний трек
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[_playList.Tracks.Count - 1].Path);
                                        TxbTrekInfo.Text =
                                            $"{_playList.Tracks[_playList.Tracks.Count - 1].Artist}-{_playList.Tracks[_playList.Tracks.Count - 1].Title}({_playList.Tracks[_playList.Tracks.Count - 1].Duration})";
                                        TextBlockBitRate.Text = $"{_playList.Tracks[_playList.Tracks.Count - 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[_playList.Tracks.Count - 1].Khz}";
                                        _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        // MediaElementPlayer.Play();
                                        //mediaPlayerIsPlaying = true;
                                    }//else

                                }//case false
                                break;

                            default:
                                break;
                        }//switch

                    }//case false:
                    break;

            }//switch

        }//Previos_Track

        #endregion

        #region События проигрывателя

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Установка максимального значения для ProgressBar и запуск потока,
            // который будет его обновлять.
            ProgressPosition.Value = 0;
            ProgressPosition.Maximum = MediaElementPlayer.NaturalDuration.TimeSpan.TotalSeconds;

            // создать новый поток исполнения, запустить этот поток
            Thread thread = new Thread(ProgressUpdater);
            thread.IsBackground = true;
            thread.Start();
        }//Player_MediaOpened

        private void _player_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (CheckBoxRepit.IsChecked)
            {
                case true:
                    {
                        MediaElementPlayer.Stop();
                        mediaPlayerIsPlaying = false;
                        TrackItem track;
                        List<TrackItem> tracks;
                        //получаем путь предыдущего трека
                        track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                        //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                        tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                        //Находим какой из треков проигрывается
                        track = tracks.Find(x => x.IsPlaying);
                        //выключаем трек в плейлисте
                        _playList.Tracks[track.Id].IsPlaying = false;

                        //если проигрываемый трек не является последний в списке, переходим на следующий трек
                        if (track.Id != _playList.Tracks.Count - 1)
                        {
                            switch (CheckBoxShaffl.IsChecked)
                            {
                                case true:
                                    {
                                        int rand = Utils.GetRand(0, _playList.Tracks.Count - 1);
                                        TxbTrekInfo.Text = $"{_playList.Tracks[rand].Artist}-{_playList.Tracks[rand].Title}({_playList.Tracks[rand].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[rand].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[rand].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[rand].Khz}";
                                        _playList.Tracks[rand].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;

                                    }//case true
                                    break;

                                case false:
                                    {
                                        TxbTrekInfo.Text = $"{_playList.Tracks[track.Id + 1].Artist}-{_playList.Tracks[track.Id + 1].Title}({_playList.Tracks[track.Id + 1].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id + 1].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[track.Id + 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[track.Id + 1].Khz}";
                                        _playList.Tracks[track.Id + 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;

                                    }//case false:
                                    break;

                            }//switch

                        }//if
                        else
                        {
                            //иначе если трек последний переходим на первый трек
                            MediaElementPlayer.Stop();
                            mediaPlayerIsPlaying = false;

                            //выключаем последний трек в плейлисте
                            _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = false;
                            //запускаем первый трек
                            TxbTrekInfo.Text = $"{_playList.Tracks[0].Artist}-{_playList.Tracks[0].Title}({_playList.Tracks[0].Duration})";
                            MediaElementPlayer.Source = new Uri(_playList.Tracks[0].Path);
                            TextBlockBitRate.Text = $"{_playList.Tracks[0].BitRate}";
                            TextBlock_kHz.Text = $"{_playList.Tracks[0].Khz}";
                            _playList.Tracks[0].IsPlaying = true;
                            LbxTreks.ItemsSource = null;
                            LbxTreks.ItemsSource = _playList.Tracks;
                            MediaElementPlayer.Play();
                            mediaPlayerIsPlaying = true;

                        }//else

                    } //case
                    break;

                case false:
                    {
                        MediaElementPlayer.Stop();
                        mediaPlayerIsPlaying = false;

                        TrackItem track;
                        List<TrackItem> tracks;

                        //получаем путь предыдущего трека
                        track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                        //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                        tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                        //Находим какой из треков проигрывается
                        track = tracks.Find(x => x.IsPlaying);
                        //выключаем трек в плейлисте
                        _playList.Tracks[track.Id].IsPlaying = false;

                        //если проигрываемый трек не является последни в списке, переходим на следующий трек
                        if (track.Id != _playList.Tracks.Count - 1)
                        {
                            switch (CheckBoxShaffl.IsChecked)
                            {
                                case true:
                                    {
                                        int rand = Utils.GetRand(0, _playList.Tracks.Count - 1);
                                        TxbTrekInfo.Text = $"{_playList.Tracks[rand].Artist}-{_playList.Tracks[rand].Title}({_playList.Tracks[rand].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[rand].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[rand].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[rand].Khz}";
                                        _playList.Tracks[rand].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;

                                    }//case true
                                    break;
                                case false:
                                    {
                                        TxbTrekInfo.Text = $"{_playList.Tracks[track.Id + 1].Artist}-{_playList.Tracks[track.Id + 1].Title}({_playList.Tracks[track.Id + 1].Duration})";
                                        MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id + 1].Path);
                                        TextBlockBitRate.Text = $"{_playList.Tracks[track.Id + 1].BitRate}";
                                        TextBlock_kHz.Text = $"{_playList.Tracks[track.Id + 1].Khz}";
                                        _playList.Tracks[track.Id + 1].IsPlaying = true;
                                        LbxTreks.ItemsSource = null;
                                        LbxTreks.ItemsSource = _playList.Tracks;
                                        MediaElementPlayer.Play();
                                        mediaPlayerIsPlaying = true;

                                    }//case false
                                    break;
                            }//switch

                        }//if
                        else
                        {
                            _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = true;
                            //если трек последний, останавливаем проигрыватель
                            MediaElementPlayer.Stop();
                            //MediaElementPlayer.Close();
                            mediaPlayerIsPlaying = false;
                        }//else

                    }//case
                    break;

            }//switch


        }//_player_MediaEnded

        #endregion

        #region Загрузка треков

        //Открыть файл
        private void Open_File(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Media files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() != true)
                return;
            TrackItem track;
            if (openFileDialog.FileNames.Length > 1)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    track = new TrackItem(openFileDialog.FileNames[i]);
                    track.Id = (_playList.Tracks.Count - 1) + 1;
                    _playList.Tracks.Add(track);
                }//for
            }//if
            else
            {
                track = new TrackItem(openFileDialog.FileName);
                track.Id = (_playList.Tracks.Count - 1) + 1;

                _playList.Tracks.Add(track);
            }//else

            stsMainUpdate();
        }//Open_ile

        //Добавить файлы  из выбранной папки в плейлист
        private void Load_Folder(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            var result = cofd.ShowDialog();
            if (result == CommonFileDialogResult.Cancel) return;

            DirectoryInfo dir = new DirectoryInfo(cofd.FileName);
            TrackItem track;

            if (dir.EnumerateDirectories().Count() > 0)
            {
                foreach (DirectoryInfo directory in dir.GetDirectories())
                {
                    foreach (FileInfo file in directory.GetFiles("*.mp3"))
                    {
                        //Добавляем полученный файл в плейлист
                        track = new TrackItem(file.FullName);
                        track.Id = (_playList.Tracks.Count - 1) + 1;
                        _playList.Tracks.Add(track);
                    }//foreach1

                }//foreach2
            }//if
            else
            {
                //Проверяем каждый mp3 файл в директории
                foreach (FileInfo file in dir.GetFiles("*.mp3"))
                {
                    //Добавляем полученный файл в плейлист
                    track = new TrackItem(file.FullName);
                    track.Id = (_playList.Tracks.Count - 1) + 1;
                    _playList.Tracks.Add(track);
                }//foreach

            }//else


            stsMainUpdate();
        }//Load_Folder

        #endregion

        #region методы с плейлистом
        //Сохранить плейлист
        private void Save_PlayList(object sender, RoutedEventArgs e)
        {
            TrackItem track = new TrackItem(MediaElementPlayer.Source.LocalPath);
            //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
            List<TrackItem> tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
            //Находим какой из треков проигрывается
            track = tracks.Find(x => x.IsPlaying);
            //выключаем трек в плейлисте
            _playList.Tracks[track.Id].IsPlaying = false;


            // Выбор файла для сохранения в станартном диалоге
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Сохраненить плейлист",
                // папка приложения
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "XML-файлы (*.xml)|*.xml|Все файлы (*.*)|*.*",
                // выбран XML из фильтра
                FilterIndex = 0,
                FileName = "playlist"
            };

            if (sfd.ShowDialog() != true)
                return;

            _playList.SaveXml(sfd.FileName);  // имя файла получено из диалога
            FileInfo info = new FileInfo(sfd.FileName);
            MessageBox.Show($"Плейлист сохранён \"{info}.xml\"",
                "Отлично!", MessageBoxButton.OK, MessageBoxImage.Information);

        }//загрузить плейлист

        //загрузить поейлист
        private void Load_PlayList(object sender, RoutedEventArgs e)
        {
            MediaElementPlayer.Stop();
            MediaElementPlayer.Source = null;

            // Выбор файла для чтения в станартном диалоге
            // если нет классов - поставить NuGet пакет
            // System.Windows.Interactivity.WPF
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Открытие коллекции заявок по квартирам",
                // папка приложения
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "XML-файлы (*.xml)|*.xml|Все файлы (*.*)|*.*",
                // выбран XML из фильтра
                FilterIndex = 0
            };

            if (ofd.ShowDialog() != true)
                return;

            // _appendix.LoadXML("bouquets.xml");
            // имя файла получаем из диалога
            _playList.LoadXml(ofd.FileName);

            LbxTreks.ItemsSource = _playList.Tracks;

            FileInfo info = new FileInfo(ofd.FileName);
            MessageBox.Show($"Плейлист загружен \"{info.Name}\"",
                "Отлично!", MessageBoxButton.OK, MessageBoxImage.Information);
            stsMainUpdate();
        }//Load_PlayList

        private void DoubleClick_Change(object sender, MouseButtonEventArgs e)
        {
            TrackItem track;

            if (MediaElementPlayer.Source != null)
            {
                //получаем путь предыдущего трека
                track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                List<TrackItem> tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                //Находим какой из треков проигрывается
                track = tracks.Find(x => x.IsPlaying);
                //выключаем трек в плейлисте
                _playList.Tracks[track.Id].IsPlaying = false;
                //запускаем выбранный трек
                track = (TrackItem)LbxTreks.SelectedItem;
                _playList.Tracks[track.Id].IsPlaying = true;
                LbxTreks.ItemsSource = null;
                LbxTreks.ItemsSource = _playList.Tracks;
                MediaElementPlayer.Source = new Uri(track.Path);
                if (MediaElementPlayer.Source == null) return;
                TxbTrekInfo.Text = $"{track.Artist}-{track.Title}({track.Duration})";
                TextBlockBitRate.Text = $"{track.BitRate}";
                TextBlock_kHz.Text = $"{track.Khz}";
                MediaElementPlayer.Play();
                mediaPlayerIsPlaying = true;

            }//if
            else
            {
                track = (TrackItem)LbxTreks.SelectedItem;
                if (LbxTreks.SelectedItem == null) return;
                track.IsPlaying = true;
                LbxTreks.ItemsSource = null;
                LbxTreks.ItemsSource = _playList.Tracks;
                
                MediaElementPlayer.Source = new Uri(track.Path);
                if (MediaElementPlayer.Source == null) return;
                TxbTrekInfo.Text = $"{track.Artist}-{track.Title}({track.Duration})";
                TextBlockBitRate.Text = $"{track.BitRate}";
                TextBlock_kHz.Text = $"{track.Khz}";
                MediaElementPlayer.Play();
                mediaPlayerIsPlaying = true;
            }//else
        }//DoubleClick_Change

        //удалить выбранный трек
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            if (LbxTreks.SelectedItem == null) return;
            if(MediaElementPlayer.Source == null) return;
            TrackItem track;
            List<TrackItem> tracks;
            //получаем путь текущего трека
            track = new TrackItem(MediaElementPlayer.Source.LocalPath);
            //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
            tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
            //Находим какой из треков проигрывается
            track = tracks.Find(x => x.IsPlaying);
            //выключаем трек в плейлисте
            //_playList.Tracks[track.Id].IsPlaying = false;

            track = (TrackItem)LbxTreks.SelectedItem;

            //если  удаляемый трек не является последним в списке , переходим на следующий трек
            if (track.Id != _playList.Tracks.Count - 1 && track.IsPlaying)
            {

                TxbTrekInfo.Text = $"{_playList.Tracks[track.Id + 1].Artist}-{_playList.Tracks[track.Id + 1].Title}({_playList.Tracks[track.Id + 1].Duration})";
                MediaElementPlayer.Source = new Uri(_playList.Tracks[track.Id + 1].Path);
                TextBlockBitRate.Text = $"{_playList.Tracks[track.Id + 1].BitRate}";
                TextBlock_kHz.Text = $"{_playList.Tracks[track.Id + 1].Khz}";
                _playList.Tracks[track.Id + 1].IsPlaying = true;
                _playList.Tracks[track.Id].IsPlaying = false;

                _playList.Tracks.Remove(track);

                for (int i = 0; i < _playList.Tracks.Count; i++)
                {
                    _playList.Tracks[i].Id = i;
                }//for

                LbxTreks.ItemsSource = null;
                LbxTreks.ItemsSource = _playList.Tracks;
                MediaElementPlayer.Play();
                mediaPlayerIsPlaying = true;

                stsMainUpdate();


            }//if
            else if (track.Id == _playList.Tracks.Count - 1 && track.IsPlaying && _playList.Tracks.Count != 1) //если удаляемый трек последний и проигрывается
            {
                //выключаем последний трек в плейлисте, переходим на предыдущий
                _playList.Tracks[_playList.Tracks.Count - 1].IsPlaying = false;
                int prev = (_playList.Tracks.Count - 1) - 1;
                //запускаем предыдущий трек
                TxbTrekInfo.Text = $"{_playList.Tracks[prev].Artist}-{_playList.Tracks[prev].Title}({_playList.Tracks[prev].Duration})";
                MediaElementPlayer.Source = new Uri(_playList.Tracks[prev].Path);
                TextBlockBitRate.Text = $"{_playList.Tracks[prev].BitRate}";
                TextBlock_kHz.Text = $"{_playList.Tracks[prev].Khz}";
                _playList.Tracks[prev].IsPlaying = true;

                _playList.Tracks.Remove(track);
                for (int i = 0; i < _playList.Tracks.Count; i++)
                {
                    _playList.Tracks[i].Id = i;
                }//for

                LbxTreks.ItemsSource = null;
                LbxTreks.ItemsSource = _playList.Tracks;
                MediaElementPlayer.Play();
                mediaPlayerIsPlaying = true;
                stsMainUpdate();

            }//if else
            else if (track.Id == _playList.Tracks.Count - 1
                     && track.IsPlaying && _playList.Tracks.Count == 1) //если удаляемый проигрываемый трек последний и остался один в списке
            {
                _playList.Tracks[track.Id].IsPlaying = false;
                _playList.Tracks.Remove(track);
                for (int i = 0; i < _playList.Tracks.Count; i++)
                {
                    _playList.Tracks[i].Id = i;
                }//for

                MediaElementPlayer.Stop();
                //MediaElementPlayer.Close();
                MediaElementPlayer.Source = null;
                LbxTreks.ItemsSource = null;
                LbxTreks.ItemsSource = _playList.Tracks;
                stsMainUpdate();

            }//else if
            else //иначе просто удаляем трек
            {
                _playList.Tracks.Remove(track);
                for (int i = 0; i < _playList.Tracks.Count; i++)
                {
                    _playList.Tracks[i].Id = i;
                }//for

                LbxTreks.ItemsSource = null;
                LbxTreks.ItemsSource = _playList.Tracks;
                stsMainUpdate();

            }//else

        }//Del_Click

        //Очистить плейлист 
        private void Dell_All(object sender, RoutedEventArgs e)
        {
            _playList.Tracks.Clear();
            MediaElementPlayer.Stop();
            MediaElementPlayer.Close();
            MediaElementPlayer.Source = null;
            stsMainUpdate();
        }//Dell_All


        #endregion

        #region Дополнительные методы
        // код, исполняющийся в отдельном потоке
        void ProgressUpdater()
        {
            while (mediaPlayerIsPlaying)
            {
                // небольшая пауза в работе потока - не каждую же миллисекунду контролировать
                Thread.Sleep(200);

                // !!! этот код приводит к падению приложения !!!
                // !!! т.к. нельзя обращаться напрямую к UI из другого потока !!! 
                // ProgressAudio.Value = _player.Position.TotalSeconds;

                // доступ к элементу интерфейса из отдельного потока исполнения 
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    (ThreadStart)(() => ProgressPosition.Value = MediaElementPlayer.Position.TotalSeconds));
            } // while
        } // ProgressUpdater

        //общее количество треков
        private void stsMainUpdate()
        {
            TbSbTotalTracks.Text = $"{_playList.Tracks.Count}";

        }//stsMainUpdate()


        #endregion

        #region События окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            _playList.LoadXml(_saveList);
            LbxTreks.ItemsSource = _playList.Tracks;
            stsMainUpdate();
        }//Window_Loaded

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MediaElementPlayer.Source != null)
            {
                TrackItem track = new TrackItem(MediaElementPlayer.Source.LocalPath);
                //находим все совпадения трека(если загружен трек имеет один и тот же путь и загружен в плейлист несколько раз)
                List<TrackItem> tracks = _playList.Tracks.ToList().FindAll(x => x.Path == track.Path);
                //Находим какой из треков проигрывается
                track = tracks.Find(x => x.IsPlaying);
                //выключаем трек в плейлисте
                _playList.Tracks[track.Id].IsPlaying = false;

                _playList.SaveXml(_saveList);  // имя файла получено из диалога
                MediaElementPlayer.Close();

            }//if
            else
            {
                _playList.SaveXml(_saveList);  // имя файла получено из диалога
                MediaElementPlayer.Close();

            }//else


        }//Window_Closing


        #endregion

        #region Реализация Drag&Drop
        private void LbxTreks_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }//if
                    FileInfo info = new FileInfo(filename);
                    if (info.Extension != ".mp3")
                    {
                        isCorrect = false;
                        break;
                    }//if
                }//foreach
            }//if
            if (isCorrect)
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;


        }//LbxTreks_OnPreviewDragEnter

        private void LbxTreks_OnPreviewDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            foreach (string fileName in filenames)
            {
                TrackItem track = new TrackItem(fileName);
                track.Id = (_playList.Tracks.Count - 1) + 1;
                _playList.Tracks.Add(track);

            }//foreach

            e.Handled = true;
            stsMainUpdate();

        }//LbxTreks_OnPreviewDrop


        #endregion

    }//MainWindow : Window
}//Task_2
