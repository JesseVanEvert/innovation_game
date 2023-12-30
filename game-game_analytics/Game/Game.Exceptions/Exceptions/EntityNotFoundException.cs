namespace Game.Exceptions.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public override string Message { get; }

        public EntityNotFoundException(string message = "Entity was not found")
        {
            Message = message;
        }
    }
}
