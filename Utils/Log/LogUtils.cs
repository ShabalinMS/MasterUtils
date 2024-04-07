using Microsoft.AspNetCore.Builder;

namespace MasterUtils.Utils.Log
{
	/// <summary>
	/// Утилита по работе с логами приложения
	/// - Создает файлы логов,
	/// - Записывает лог в файлы (common, error)
	/// </summary>
	public class LogUtils
	{

		#region Private params

		/// <summary>
		/// Блокировщих записи в файл Common
		/// </summary>
		private readonly object _balanceLockCommon = new object();

		/// <summary>
		/// Блокировщик записи в файл Error
		/// </summary>
		private readonly object _balanceLockError = new object(); 

		/// <summary>
		/// Записывать лог в консоль
		/// </summary>
		private bool _isWriteConsole = false;

		/// <summary>
		/// Файл для общей информации
		/// </summary>
		private FileInfo _fileCommon;

		/// <summary>
		/// Файл для ошибок и исключений
		/// </summary>
		private FileInfo _fileError;

		/// <summary>
		/// <see cref="WebApplication"/>
		/// </summary>
		private WebApplication _webApplication;

		#endregion

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="path">Путь к папке, куда записывать логи</param>
		/// <param name="IsWriteConsole">Записывать логи в консоль</param>
		/// <param name="app"><see cref="WebApplication"/>Конфиг</param>
		public LogUtils(DirectoryInfo path, bool IsWriteConsole, WebApplication app)
		{
			_isWriteConsole = IsWriteConsole;
			Task.Run(() =>initLog(path));
		}

		#region Methods Public

		/// <summary>
		/// Запись в общий лог
		/// </summary>
		/// <param name="value">Записываемое значение</param>
		/// <returns></returns>
		public void WriteCommon(params string[] value)
		{
			balanceWriteInCommon(value);
		}

		/// <summary>
		/// Запись в ошибку
		/// </summary>
		/// <param name="value">Записываемое значние</param>
		public void WriteError(string value)
		{
			balanceWriteInError(value.ToString());
		}

		/// <summary>
		/// Запись в ошибку
		/// </summary>
		/// <param name="value">Записываемое значение</param>
		public void WriteError(Exception value)
		{
			balanceWriteInError(value.ToString());
		}

		#endregion

		#region Methods private

		/// <summary>
		/// Балансер для файла Common
		/// </summary>
		/// <param name="values">Записываемые значения</param>
		private void balanceWriteInCommon(params string[] values)
		{
			lock(_balanceLockCommon)
			{
				WriteLog(_fileCommon, values.ToString());
			}
		}

		/// <summary>
		/// Балансер для записи в файл Error 
		/// </summary>
		/// <param name="values"></param>
		private void balanceWriteInError(params string[] values)
		{
			lock(_balanceLockError)
			{
				WriteLog(_fileError, values.ToString());
			}
		}

		/// <summary>
		/// Запись лога в файл
		/// </summary>
		/// <param name="values">Значения, которое нужно записать</param>
		/// <param name="file">файл</param>
		/// <returns></returns>
		private async Task WriteLog(FileInfo file, params string[] values)
		{
			StreamWriter stream = new StreamWriter(file.FullName);

            foreach (var item in values)
            {
				WriteInConsole(file, item);
				stream.WriteLine(item);
			}
           
			stream.Close();
		}

		/// <summary>
		/// Запись лога в консоль
		/// </summary>
		/// <param name="file">Файл для записи</param>
		/// <param name="value">Значение для записи</param>
		private void WriteInConsole(FileInfo file, string value)
		{
			if(_isWriteConsole)
			{
				if (file.Equals(_fileCommon))
				{
					_webApplication.Logger.LogInformation(value);
				}
				else if (file.Equals(_fileError))
				{
					_webApplication.Logger.LogError(value);
				}
			}
		}

		/// <summary>
		/// Инициализация фалов для логов
		/// </summary>
		/// <param name="path">Путь в логам</param>
		private async Task initLog(DirectoryInfo path)
		{
			if(string.IsNullOrWhiteSpace(path.FullName))
			{
				var test = LogConstants.PATHDEFAULT;
			}
			if (!path.Exists)
			{
				path.Create();
			} else {
				await Task.WhenAll(
					initFileAsync(_fileCommon = new FileInfo(string.Concat(path.FullName, "\\", LogConstants.NAMEFILECOMMON))),
					initFileAsync(_fileError = new FileInfo(string.Concat(path.FullName, "\\", LogConstants.NAMEFILEERROR)))
				);
			}
		}

		/// <summary>
		/// Асинхронная иннициализация файлов и фиксация запуска приложения
		/// </summary>
		/// <param name="file">Файл</param>
		/// <returns></returns>
		private async Task initFileAsync(FileInfo file)
		{
			if (!file.Exists)
			{
				_fileCommon.Create();
			}

			string messageInit = $"Init file {0}";

			if(file.Equals(_fileCommon))
			{
				await WriteLog(_fileCommon, string.Format(messageInit, LogConstants.NAMEFILECOMMON), $"Start application");
			} else if(file.Equals(_fileError))
			{
				await WriteLog(_fileError, string.Format(messageInit, LogConstants.NAMEFILEERROR));
			}
		}

		#endregion
	}
}
