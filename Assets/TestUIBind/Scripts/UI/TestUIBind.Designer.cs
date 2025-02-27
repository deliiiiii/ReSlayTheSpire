using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:5c95a143-b7a4-44ff-9295-fdf742a8c9c0
	public partial class TestUIBind
	{
		public const string Name = "TestUIBind";
		
		[SerializeField]
		public UnityEngine.UI.Text TextEnemyCurHP;
		[SerializeField]
		public UnityEngine.UI.Text TextEnemyMaxHP;
		/// <summary>
		/// qqq
		/// </summary>
		[SerializeField]
		public UnityEngine.UI.Button ButtonFight;
		[SerializeField]
		public UnityEngine.UI.Text TextFight;
		
		private TestUIBindData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TextEnemyCurHP = null;
			TextEnemyMaxHP = null;
			ButtonFight = null;
			TextFight = null;
			
			mData = null;
		}
		
		public TestUIBindData Data
		{
			get
			{
				return mData;
			}
		}
		
		TestUIBindData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new TestUIBindData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
