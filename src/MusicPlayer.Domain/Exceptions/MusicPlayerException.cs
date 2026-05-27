using System;

namespace MusicPlayer.Domain.Exceptions
{
    public class MusicPlayerException : Exception
    {
        public MusicPlayerException()
        {
        }

        public MusicPlayerException(string message)
            : base(message)
        {
        }

        public MusicPlayerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class InvalidTrackException : MusicPlayerException
    {
        public InvalidTrackException()
        {
        }

        public InvalidTrackException(string message)
            : base(message)
        {
        }

        public InvalidTrackException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class StorageException : MusicPlayerException
    {
        public StorageException()
        {
        }

        public StorageException(string message)
            : base(message)
        {
        }

        public StorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
