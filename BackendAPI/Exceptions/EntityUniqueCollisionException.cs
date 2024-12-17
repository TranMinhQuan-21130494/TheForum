namespace BackendAPI.Exceptions {
    public class EntityUniqueCollisionException : Exception{
        public EntityUniqueCollisionException(string message) : base(message) { }
    }
}
