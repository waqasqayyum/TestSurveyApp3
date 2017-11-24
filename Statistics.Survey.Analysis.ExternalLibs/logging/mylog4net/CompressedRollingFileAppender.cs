using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using System.Collections;
using log4net.Util;
using log4net.Core;
using System.IO;
using System.Globalization;
using System.IO.Compression;

namespace Utilities.Logger.mylog4net
{
	public class CompressedRollingFileAppender : FileAppender
	{
		// Fields
		private string m_baseFileName;
		private int m_countDirection = -1;
		private int m_curSizeRollBackups = 0;
		private string m_datePattern = ".yyyy-MM-dd";
		private RollingFileAppender.IDateTime m_dateTime = null;
		private long m_maxFileSize = 0xa00000L;
		private int m_maxSizeRollBackups = 0;
		private DateTime m_nextCheck = DateTime.MaxValue;
		private DateTime m_now;
		private bool m_rollDate = true;
		private RollingFileAppender.RollingMode m_rollingStyle = RollingFileAppender.RollingMode.Composite;
		private RollPoint m_rollPoint;
		private bool m_rollSize = true;
		private string m_scheduledFilename = null;
		private bool m_staticLogFileName = true;
		private static readonly DateTime s_date1970 = new DateTime(0x7b2, 1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressedRollingFileAppender"/> class.
        /// </summary>
		public CompressedRollingFileAppender()
		{
			this.m_dateTime = new DefaultDateTime();
		}

        /// <summary>
        /// Activate the options on the file appender.
        /// </summary>
		public override void ActivateOptions()
		{
			if (this.m_rollDate && (this.m_datePattern != null))
			{
				this.m_now = this.m_dateTime.Now;
				this.m_rollPoint = this.ComputeCheckPeriod(this.m_datePattern);
				if (this.m_rollPoint == RollPoint.InvalidRollPoint)
				{
					throw new ArgumentException("Invalid RollPoint, unable to parse [" + this.m_datePattern + "]");
				}
				this.m_nextCheck = this.NextCheckDate(this.m_now, this.m_rollPoint);
			}
			else if (this.m_rollDate)
			{
				this.ErrorHandler.Error("Either DatePattern or rollingStyle options are not set for [" + base.Name + "].");
			}
			if (base.SecurityContext == null)
			{
				base.SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			using (base.SecurityContext.Impersonate(this))
			{
				base.File = FileAppender.ConvertToFullPath(base.File.Trim());
				this.m_baseFileName = base.File;
			}
			if ((this.m_rollDate && (this.File != null)) && (this.m_scheduledFilename == null))
			{
				this.m_scheduledFilename = this.File + this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
			}
			this.ExistingInit();
			base.ActivateOptions();
		}

        /// <summary>
        /// Adjusts the file before append.
        /// </summary>
		protected virtual void AdjustFileBeforeAppend()
		{
			if (this.m_rollDate)
			{
				DateTime n = this.m_dateTime.Now;
				if (n >= this.m_nextCheck)
				{
					this.m_now = n;
					this.m_nextCheck = this.NextCheckDate(this.m_now, this.m_rollPoint);
					this.RollOverTime(true);
				}
			}
			if (this.m_rollSize && ((this.File != null) && (((CountingQuietTextWriter)base.QuietWriter).Count >= this.m_maxFileSize)))
			{
				this.RollOverSize();
			}
		}

        /// <summary>
        /// This method is called by the <see cref="M:log4net.Appender.AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent[])"/>
        /// method.
        /// </summary>
        /// <param name="loggingEvents">The array of events to log.</param>
		protected override void Append(LoggingEvent[] loggingEvents)
		{
			this.AdjustFileBeforeAppend();
			base.Append(loggingEvents);
		}

        /// <summary>
        /// This method is called by the <see cref="M:log4net.Appender.AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)"/>
        /// method.
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
		protected override void Append(LoggingEvent loggingEvent)
		{
			this.AdjustFileBeforeAppend();
			base.Append(loggingEvent);
		}

        /// <summary>
        /// Computes the check period.
        /// </summary>
        /// <param name="datePattern">The date pattern.</param>
        /// <returns></returns>
		private RollPoint ComputeCheckPeriod(string datePattern)
		{
			string r0 = s_date1970.ToString(datePattern, DateTimeFormatInfo.InvariantInfo);
			for (int i = 0; i <= 5; i++)
			{
				string r1 = this.NextCheckDate(s_date1970, (RollPoint)i).ToString(datePattern, DateTimeFormatInfo.InvariantInfo);
				LogLog.Debug(string.Concat(new object[] { "RollingFileAppender: Type = [", i, "], r0 = [", r0, "], r1 = [", r1, "]" }));
				if (!(((r0 == null) || (r1 == null)) || r0.Equals(r1)))
				{
					return (RollPoint)i;
				}
			}
			return RollPoint.InvalidRollPoint;
		}

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
		protected void DeleteFile(string fileName)
		{
			if (this.FileExists(fileName))
			{
				IDisposable temp;
				string fileToDelete = fileName;
				string tempFileName = string.Concat(new object[] { fileName, ".", Environment.TickCount, ".DeletePending" });
				try
				{
					using (temp = base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fileName, tempFileName);
					}
					fileToDelete = tempFileName;
				}
				catch (Exception moveEx)
				{
					LogLog.Debug("RollingFileAppender: Exception while moving file to be deleted [" + fileName + "] -> [" + tempFileName + "]", moveEx);
				}
				try
				{
					using (temp = base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Delete(fileToDelete);
					}
					LogLog.Debug("RollingFileAppender: Deleted file [" + fileName + "]");
				}
				catch (Exception deleteEx)
				{
					if (fileToDelete == fileName)
					{
						this.ErrorHandler.Error("Exception while deleting file [" + fileToDelete + "]", deleteEx, ErrorCode.GenericFailure);
					}
					else
					{
						LogLog.Debug("RollingFileAppender: Exception while deleting temp file [" + fileToDelete + "]", deleteEx);
					}
				}
			}
		}

        /// <summary>
        /// Determines the cur size roll backups.
        /// </summary>
		private void DetermineCurSizeRollBackups()
		{
			this.m_curSizeRollBackups = 0;
			string fullPath = null;
			string fileName = null;
			using (base.SecurityContext.Impersonate(this))
			{
				fullPath = Path.GetFullPath(this.m_baseFileName);
				fileName = Path.GetFileName(fullPath);
			}
			ArrayList arrayFiles = this.GetExistingFiles(fullPath);
			this.InitializeRollBackups(fileName, arrayFiles);
			LogLog.Debug("RollingFileAppender: curSizeRollBackups starts at [" + this.m_curSizeRollBackups + "]");
		}

        /// <summary>
        /// Existings the init.
        /// </summary>
		protected void ExistingInit()
		{
			this.DetermineCurSizeRollBackups();
			this.RollOverIfDateBoundaryCrossing();
			if (!base.AppendToFile)
			{
				bool fileExists = false;
				string fileName = this.GetNextOutputFileName(this.m_baseFileName);
				using (base.SecurityContext.Impersonate(this))
				{
					fileExists = System.IO.File.Exists(fileName);
				}
				if (fileExists)
				{
					if (this.m_maxSizeRollBackups == 0)
					{
						LogLog.Debug("RollingFileAppender: Output file [" + fileName + "] already exists. MaxSizeRollBackups is 0; cannot roll. Overwriting existing file.");
					}
					else
					{
						LogLog.Debug("RollingFileAppender: Output file [" + fileName + "] already exists. Not appending to file. Rolling existing file out of the way.");
						this.RollOverRenameFiles(fileName);
					}
				}
			}
		}

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
		protected bool FileExists(string path)
		{
			using (base.SecurityContext.Impersonate(this))
			{
				return System.IO.File.Exists(path);
			}
		}

        /// <summary>
        /// Gets the existing files.
        /// </summary>
        /// <param name="baseFilePath">The base file path.</param>
        /// <returns></returns>
		private ArrayList GetExistingFiles(string baseFilePath)
		{
			ArrayList alFiles = new ArrayList();
			string directory = null;
			using (base.SecurityContext.Impersonate(this))
			{
				string fullPath = Path.GetFullPath(baseFilePath);
				directory = Path.GetDirectoryName(fullPath);
				if (Directory.Exists(directory))
				{
					string baseFileName = Path.GetFileName(fullPath);
					string[] files = Directory.GetFiles(directory, GetWildcardPatternForFile(baseFileName));
					if (files != null)
					{
						for (int i = 0; i < files.Length; i++)
						{
							string curFileName = Path.GetFileName(files[i]);
							if (curFileName.StartsWith(baseFileName))
							{
								alFiles.Add(curFileName);
							}
						}
					}
				}
			}
			LogLog.Debug("RollingFileAppender: Searched for existing files in [" + directory + "]");
			return alFiles;
		}

        /// <summary>
        /// Gets the name of the next output file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
		protected string GetNextOutputFileName(string fileName)
		{
			if (!this.m_staticLogFileName)
			{
				fileName = fileName.Trim();
				if (this.m_rollDate)
				{
					fileName = fileName + this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
				}
				if (this.m_countDirection >= 0)
				{
					fileName = fileName + '.' + this.m_curSizeRollBackups;
				}
			}
			return fileName;
		}

        /// <summary>
        /// Gets the wildcard pattern for file.
        /// </summary>
        /// <param name="baseFileName">Name of the base file.</param>
        /// <returns></returns>
		private static string GetWildcardPatternForFile(string baseFileName)
		{
			return (baseFileName + '*');
		}

        /// <summary>
        /// Initializes from one file.
        /// </summary>
        /// <param name="baseFile">The base file.</param>
        /// <param name="curFileName">Name of the cur file.</param>
		private void InitializeFromOneFile(string baseFile, string curFileName)
		{
			if (curFileName.StartsWith(baseFile) && !curFileName.Equals(baseFile))
			{
				int index = curFileName.LastIndexOf(".");
				if (-1 != index)
				{
					if (this.m_staticLogFileName)
					{
						int endLength = curFileName.Length - index;
						if ((baseFile.Length + endLength) != curFileName.Length)
						{
							return;
						}
					}
					if ((this.m_rollDate && !this.m_staticLogFileName) && !curFileName.StartsWith(baseFile + this.m_dateTime.Now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo)))
					{
						LogLog.Debug("RollingFileAppender: Ignoring file [" + curFileName + "] because it is from a different date period");
					}
					else
					{
						try
						{
							int backup;
							if (SystemInfo.TryParse(curFileName.Substring(index + 1), out backup) && (backup > this.m_curSizeRollBackups))
							{
								if (0 != this.m_maxSizeRollBackups)
								{
									if (-1 == this.m_maxSizeRollBackups)
									{
										this.m_curSizeRollBackups = backup;
									}
									else if (this.m_countDirection >= 0)
									{
										this.m_curSizeRollBackups = backup;
									}
									else if (backup <= this.m_maxSizeRollBackups)
									{
										this.m_curSizeRollBackups = backup;
									}
								}
								LogLog.Debug(string.Concat(new object[] { "RollingFileAppender: File name [", curFileName, "] moves current count to [", this.m_curSizeRollBackups, "]" }));
							}
						}
						catch (FormatException)
						{
							LogLog.Debug("RollingFileAppender: Encountered a backup file not ending in .x [" + curFileName + "]");
						}
					}
				}
			}
		}

        /// <summary>
        /// Initializes the roll backups.
        /// </summary>
        /// <param name="baseFile">The base file.</param>
        /// <param name="arrayFiles">The array files.</param>
		private void InitializeRollBackups(string baseFile, ArrayList arrayFiles)
		{
			if (null != arrayFiles)
			{
				string baseFileLower = baseFile.ToLower(CultureInfo.InvariantCulture);
				foreach (string curFileName in arrayFiles)
				{
					this.InitializeFromOneFile(baseFileLower, curFileName.ToLower(CultureInfo.InvariantCulture));
				}
			}
		}

        /// <summary>
        /// Nexts the check date.
        /// </summary>
        /// <param name="currentDateTime">The current date time.</param>
        /// <param name="rollPoint">The roll point.</param>
        /// <returns></returns>
		protected DateTime NextCheckDate(DateTime currentDateTime,  RollPoint rollPoint)
		{
			DateTime current = currentDateTime;
			switch (rollPoint)
			{
				case RollPoint.TopOfMinute:
					current = current.AddMilliseconds((double)-current.Millisecond);
					return current.AddSeconds((double)-current.Second).AddMinutes(1.0);

				case RollPoint.TopOfHour:
					current = current.AddMilliseconds((double)-current.Millisecond);
					current = current.AddSeconds((double)-current.Second);
					return current.AddMinutes((double)-current.Minute).AddHours(1.0);

				case RollPoint.HalfDay:
					current = current.AddMilliseconds((double)-current.Millisecond);
					current = current.AddSeconds((double)-current.Second);
					current = current.AddMinutes((double)-current.Minute);
					if (current.Hour >= 12)
					{
						return current.AddHours((double)-current.Hour).AddDays(1.0);
					}
					return current.AddHours((double)(12 - current.Hour));

				case RollPoint.TopOfDay:
					current = current.AddMilliseconds((double)-current.Millisecond);
					current = current.AddSeconds((double)-current.Second);
					current = current.AddMinutes((double)-current.Minute);
					return current.AddHours((double)-current.Hour).AddDays(1.0);

				case RollPoint.TopOfWeek:
					current = current.AddMilliseconds((double)-current.Millisecond);
					current = current.AddSeconds((double)-current.Second);
					current = current.AddMinutes((double)-current.Minute);
					current = current.AddHours((double)-current.Hour);
					return current.AddDays((double)(7 - current.DayOfWeek));

				case RollPoint.TopOfMonth:
					current = current.AddMilliseconds((double)-current.Millisecond);
					current = current.AddSeconds((double)-current.Second);
					current = current.AddMinutes((double)-current.Minute);
					current = current.AddHours((double)-current.Hour);
					return current.AddDays((double)(1 - current.Day)).AddMonths(1);
			}
			return current;
		}

        /// <summary>
        /// Sets and <i>opens</i> the file where the log output will go. The specified file must be writable.
        /// </summary>
        /// <param name="fileName">The path to the log file. Must be a fully qualified path.</param>
        /// <param name="append">If true will append to fileName. Otherwise will truncate fileName</param>
		protected override void OpenFile(string fileName, bool append)
		{
			lock (this)
			{
				fileName = this.GetNextOutputFileName(fileName);
				long currentCount = 0L;
				if (append)
				{
					using (base.SecurityContext.Impersonate(this))
					{
						if (System.IO.File.Exists(fileName))
						{
							currentCount = new FileInfo(fileName).Length;
						}
					}
				}
				else if (LogLog.IsErrorEnabled && ((this.m_maxSizeRollBackups != 0) && this.FileExists(fileName)))
				{
					LogLog.Error("RollingFileAppender: INTERNAL ERROR. Append is False but OutputFile [" + fileName + "] already exists.");
				}
				if (!this.m_staticLogFileName)
				{
					this.m_scheduledFilename = fileName;
				}
				base.OpenFile(fileName, append);
				((CountingQuietTextWriter)base.QuietWriter).Count = currentCount;
			}
		}

        /// <summary>
        /// Rolls the file.
        /// </summary>
        /// <param name="fromFile">From file.</param>
        /// <param name="toFile">To file.</param>
		protected void RollFile(string fromFile, string toFile)
		{
			if (this.FileExists(fromFile))
			{
				this.DeleteFile(toFile);
				try
				{
					LogLog.Debug("RollingFileAppender: Moving [" + fromFile + "] -> [" + toFile + "]");
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fromFile, toFile);

						#region Code Changed By Zeeshan Umar for Compression
                        if (!toFile.Contains(".zip"))
						{
                            //ZipArchive.CreateZipFile(toFile+".zip",new[]{toFile}.ToList());
							FileStream fs = new FileStream(toFile, FileMode.Open);
							byte[] input = new byte[fs.Length];
							fs.Read(input, 0, input.Length);
							fs.Close();

							FileStream fsOutput = new FileStream(toFile + ".gz", FileMode.Create, FileAccess.Write);
							GZipStream zip = new GZipStream(fsOutput, CompressionMode.Compress);

							zip.Write(input, 0, input.Length);
							zip.Close();
							fsOutput.Close();
							DeleteFile(toFile);
						}
						#endregion
					}
				}
				catch (Exception moveEx)
				{
					this.ErrorHandler.Error("Exception while rolling file [" + fromFile + "] -> [" + toFile + "]", moveEx, ErrorCode.GenericFailure);
				}
			}
			else
			{
				LogLog.Warn("RollingFileAppender: Cannot RollFile [" + fromFile + "] -> [" + toFile + "]. Source does not exist");
			}
		}

        /// <summary>
        /// Rolls the over if date boundary crossing.
        /// </summary>
		private void RollOverIfDateBoundaryCrossing()
		{
			if ((this.m_staticLogFileName && this.m_rollDate) && this.FileExists(this.m_baseFileName))
			{
				DateTime last;
				using (base.SecurityContext.Impersonate(this))
				{
					last = System.IO.File.GetLastWriteTime(this.m_baseFileName);
				}
				LogLog.Debug("RollingFileAppender: [" + last.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo) + "] vs. [" + this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo) + "]");
				if (!last.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo).Equals(this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo)))
				{
					this.m_scheduledFilename = this.m_baseFileName + last.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
					LogLog.Debug("RollingFileAppender: Initial roll over to [" + this.m_scheduledFilename + "]");
					this.RollOverTime(false);
					LogLog.Debug("RollingFileAppender: curSizeRollBackups after rollOver at [" + this.m_curSizeRollBackups + "]");
				}
			}
		}

        /// <summary>
        /// Rolls the over rename files.
        /// </summary>
        /// <param name="baseFileName">Name of the base file.</param>
		protected void RollOverRenameFiles(string baseFileName)
		{
			if (this.m_maxSizeRollBackups != 0)
			{
				if (this.m_countDirection < 0)
				{
					if (this.m_curSizeRollBackups == this.m_maxSizeRollBackups)
					{
						this.DeleteFile(baseFileName + '.' + this.m_maxSizeRollBackups);
						this.m_curSizeRollBackups--;
					}
					for (int i = this.m_curSizeRollBackups; i >= 1; i--)
					{
						this.RollFile(baseFileName + "." + i, baseFileName + '.' + (i + 1));
					}
					this.m_curSizeRollBackups++;
					this.RollFile(baseFileName, baseFileName + ".1");
				}
				else
				{
					if ((this.m_curSizeRollBackups >= this.m_maxSizeRollBackups) && (this.m_maxSizeRollBackups > 0))
					{
						int oldestFileIndex = this.m_curSizeRollBackups - this.m_maxSizeRollBackups;
						if (this.m_staticLogFileName)
						{
							oldestFileIndex++;
						}
						string archiveFileBaseName = baseFileName;
						if (!this.m_staticLogFileName)
						{
							int lastDotIndex = archiveFileBaseName.LastIndexOf(".");
							if (lastDotIndex >= 0)
							{
								archiveFileBaseName = archiveFileBaseName.Substring(0, lastDotIndex);
							}
						}
						this.DeleteFile(archiveFileBaseName + '.' + oldestFileIndex);
					}
					if (this.m_staticLogFileName)
					{
						this.m_curSizeRollBackups++;
						this.RollFile(baseFileName, baseFileName + '.' + this.m_curSizeRollBackups);
					}
				}
			}
		}

        /// <summary>
        /// Rolls the size of the over.
        /// </summary>
		protected void RollOverSize()
		{
			base.CloseFile();
			LogLog.Debug("RollingFileAppender: rolling over count [" + ((CountingQuietTextWriter)base.QuietWriter).Count + "]");
			LogLog.Debug("RollingFileAppender: maxSizeRollBackups [" + this.m_maxSizeRollBackups + "]");
			LogLog.Debug("RollingFileAppender: curSizeRollBackups [" + this.m_curSizeRollBackups + "]");
			LogLog.Debug("RollingFileAppender: countDirection [" + this.m_countDirection + "]");
			this.RollOverRenameFiles(this.File);
			if (!(this.m_staticLogFileName || (this.m_countDirection < 0)))
			{
				this.m_curSizeRollBackups++;
			}
			this.SafeOpenFile(this.m_baseFileName, false);
		}

        /// <summary>
        /// Rolls the over time.
        /// </summary>
        /// <param name="fileIsOpen">if set to <c>true</c> [file is open].</param>
		protected void RollOverTime(bool fileIsOpen)
		{
			if (this.m_staticLogFileName)
			{
				if (this.m_datePattern == null)
				{
					this.ErrorHandler.Error("Missing DatePattern option in rollOver().");
					return;
				}
				string dateFormat = this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
				if (this.m_scheduledFilename.Equals(this.File + dateFormat))
				{
					this.ErrorHandler.Error("Compare " + this.m_scheduledFilename + " : " + this.File + dateFormat);
					return;
				}
				if (fileIsOpen)
				{
					base.CloseFile();
				}
				for (int i = 1; i <= this.m_curSizeRollBackups; i++)
				{
					string from = this.File + '.' + i;
					string to = this.m_scheduledFilename + '.' + i;
					this.RollFile(from, to);
				}
				this.RollFile(this.File, this.m_scheduledFilename);
			}
			this.m_curSizeRollBackups = 0;
			this.m_scheduledFilename = this.File + this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
			if (fileIsOpen)
			{
				this.SafeOpenFile(this.m_baseFileName, false);
			}
		}

        /// <summary>
        /// Sets the quiet writer being used.
        /// </summary>
        /// <param name="writer">the writer over the file stream that has been opened for writing</param>
		protected override void SetQWForFiles(TextWriter writer)
		{
			base.QuietWriter = new CountingQuietTextWriter(writer, this.ErrorHandler);
		}

        /// <summary>
        /// Gets or sets the count direction.
        /// </summary>
        /// <value>
        /// The count direction.
        /// </value>
		public int CountDirection
		{
			get
			{
				return this.m_countDirection;
			}
			set
			{
				this.m_countDirection = value;
			}
		}

        /// <summary>
        /// Gets or sets the date pattern.
        /// </summary>
        /// <value>
        /// The date pattern.
        /// </value>
		public string DatePattern
		{
			get
			{
				return this.m_datePattern;
			}
			set
			{
				this.m_datePattern = value;
			}
		}

        /// <summary>
        /// Gets or sets the size of the max file.
        /// </summary>
        /// <value>
        /// The size of the max file.
        /// </value>
		public long MaxFileSize
		{
			get
			{
				return this.m_maxFileSize;
			}
			set
			{
				this.m_maxFileSize = value;
			}
		}

        /// <summary>
        /// Gets or sets the maximum size of the file.
        /// </summary>
        /// <value>
        /// The maximum size of the file.
        /// </value>
		public string MaximumFileSize
		{
			get
			{
				return this.m_maxFileSize.ToString(NumberFormatInfo.InvariantInfo);
			}
			set
			{
				this.m_maxFileSize = OptionConverter.ToFileSize(value, this.m_maxFileSize + 1L);
			}
		}

        /// <summary>
        /// Gets or sets the max size roll backups.
        /// </summary>
        /// <value>
        /// The max size roll backups.
        /// </value>
		public int MaxSizeRollBackups
		{
			get
			{
				return this.m_maxSizeRollBackups;
			}
			set
			{
				this.m_maxSizeRollBackups = value;
			}
		}

        /// <summary>
        /// Gets or sets the rolling style.
        /// </summary>
        /// <value>
        /// The rolling style.
        /// </value>
		public RollingFileAppender.RollingMode RollingStyle
		{
			get
			{
				return this.m_rollingStyle;
			}
			set
			{
				this.m_rollingStyle = value;
				switch (this.m_rollingStyle)
				{
					case RollingFileAppender.RollingMode.Once:
						this.m_rollDate = false;
						this.m_rollSize = false;
						base.AppendToFile = false;
						break;

					case RollingFileAppender.RollingMode.Size:
						this.m_rollDate = false;
						this.m_rollSize = true;
						break;

					case RollingFileAppender.RollingMode.Date:
						this.m_rollDate = true;
						this.m_rollSize = false;
						break;

					case RollingFileAppender.RollingMode.Composite:
						this.m_rollDate = true;
						this.m_rollSize = true;
						break;
				}
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether [static log file name].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [static log file name]; otherwise, <c>false</c>.
        /// </value>
		public bool StaticLogFileName
		{
			get
			{
				return this.m_staticLogFileName;
			}
			set
			{
				this.m_staticLogFileName = value;
			}
		}

		// Nested Types
		private class DefaultDateTime : RollingFileAppender.IDateTime
		{
			// Properties
			public DateTime Now
			{
				get
				{
					return DateTime.Now;
				}
			}
		}

	}
}
