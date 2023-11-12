namespace Food_Delivery.Models
{
    public static class constants
    {
        public const string cookie_loggeduser_id = "cookie_loggeduser_keycode-first";
        public const string cookie_loggeduser_passcode = "cookie_loggeduser_keycode-second";
        public enum statuses
        {
            admin = 3,
            user = 1,
            delivery_man = 2
        }
        public static String[] default_controller = { "Home", "User", "Cur" };
    }
}
