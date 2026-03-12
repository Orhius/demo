namespace shoesMic.Models
{
    /// <summary>
    /// Пользователь системы (клиент или администратор).
    /// Соответствует таблице <c>users</c> в базе данных.
    /// </summary>
    public class User
    {
        /// <summary>Уникальный идентификатор пользователя (первичный ключ).</summary>
        public int UserId { get; set; }

        /// <summary>Идентификатор роли пользователя (внешний ключ → <see cref="Role"/>).</summary>
        public int RoleId { get; set; }

        /// <summary>Полное имя пользователя.</summary>
        public string FullName { get; set; }

        /// <summary>Логин для входа в систему.</summary>
        public string Login { get; set; }

        /// <summary>Пароль пользователя.</summary>
        public string Password { get; set; }

        /// <summary>Навигационное свойство — роль пользователя.</summary>
        public Role Role { get; set; }
    }
}

