namespace BSTServer.Hosting
{
    public interface IRegexObj<out T>
    {
        T Result { get; }
        bool Success { get; }
        void Match(string line);
    }
}