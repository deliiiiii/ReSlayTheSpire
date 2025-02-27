using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class TestUIBindData : UIPanelData
	{
		public EnemyViewModel EnemyViewModel;
	}
	public partial class TestUIBind : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as TestUIBindData ?? new TestUIBindData();
			// please add init code here
			
			mData.EnemyViewModel.OnEnemyHPChange += () =>
			{
				MyDebug.Log("OnEnemyHPChange" + mData.EnemyViewModel.CurHP);
				TextEnemyCurHP.text = mData.EnemyViewModel.CurHP.ToString();
			};
			ButtonFight.onClick.AddListener(() =>
			{
				mData.EnemyViewModel.Fight();
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			TextEnemyMaxHP.text = mData.EnemyViewModel.MaxHP.ToString();
			TextEnemyCurHP.text = mData.EnemyViewModel.CurHP.ToString();
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
