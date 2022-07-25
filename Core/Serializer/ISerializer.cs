namespace Core.Serializer
{
    public interface ISerializer
    {
        public void Enqueue(Task task);
    }
}