namespace UGB.Proyecto.Final.Entities
{
    public class users_roles
    {
        public int user_id { get; set; }
        public int role_id { get; set; }
        public roles role { get; set; } = new roles();
    }
}