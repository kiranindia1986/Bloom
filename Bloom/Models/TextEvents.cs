using System;
using System.ComponentModel;

namespace Bloom.Models
{
    public class TextEvent : INotifyPropertyChanged
    {
        private float _time;
        private string _text;
        private TextType _speechType;
        private FaceType _face;
        private CharacterType _character;
        private bool _waitForInput;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public float Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(nameof(Time)); }
        }

        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        public TextType SpeechType
        {
            get => _speechType;
            set { _speechType = value; OnPropertyChanged(nameof(SpeechType)); }
        }

        public FaceType Face
        {
            get => _face;
            set { _face = value; OnPropertyChanged(nameof(Face)); }
        }

        public CharacterType Character
        {
            get => _character;
            set { _character = value; OnPropertyChanged(nameof(Character)); }
        }

        public bool WaitForInput
        {
            get => _waitForInput;
            set { _waitForInput = value; OnPropertyChanged(nameof(WaitForInput)); }
        }

        public TextEvent() { }

        public TextEvent(float time, string text, TextType speech, FaceType face)
        {
            Time = time;
            Text = text;
            SpeechType = speech;
            Face = face;
        }
    }
}
