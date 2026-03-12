namespace shoesMic.Models
{
    /// <summary>
    /// Роль пользователя в системе (например: "Администратор", "Клиент").
    /// Соответствует таблице <c>roles</c> в базе данных.
    /// </summary>
    public class Role
    {
        /// <summary>Уникальный идентификатор роли (первичный ключ).</summary>
        public int RoleId { get; set; }

        /// <summary>Название роли.</summary>
        public string RoleName { get; set; }
    }
}

