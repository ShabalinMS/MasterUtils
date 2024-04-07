using Microsoft.AspNetCore.Http;
using System.Xml.Linq;

namespace MasterUtils.Utils.Log
{
	/// <summary>
	/// Константы для логов
	/// </summary>
	public static class LogConstants
	{
		/// <summary>
		/// Путь по умолчанию
		/// </summary>
		public const string PATHDEFAULT = "Log\\";

		/// <summary>
		/// Наименование файла для общих логов
		/// </summary>
		public const string NAMEFILECOMMON = "Common.log";

		/// <summary>
		/// Наименование файла ошибок
		/// </summary>
		public const string NAMEFILEERROR = "Error.log";

		/// <summary>
		/// Паттер записи в лог
		/// </summary>
		public static string PatterWriteLog => $"{DateTime.Now.ToLocalTime()} {0}";
	}
}
