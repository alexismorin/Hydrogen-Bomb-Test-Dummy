// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public enum AdditionalLineType
	{
		Include,
		Define,
		Pragma,
		Custom
	}

	[Serializable]
	public class AdditionalDirectiveContainer : ScriptableObject
	{
		private const string IncludeFormat = "#include \"{0}\"";
		private const string PragmaFormat = "#pragma {0}";
		private const string DefineFormat = "#define {0}";

		public AdditionalLineType LineType;
		public string LineValue;
		public AdditionalDirectiveContainer()
		{
			LineType = AdditionalLineType.Include;
			LineValue = string.Empty;
		}

		public AdditionalDirectiveContainer( AdditionalLineType lineType, string lineValue )
		{
			LineType = lineType;
			LineValue = lineValue;
		}

		public string FormattedValue
		{
			get
			{
				switch( LineType )
				{
					case AdditionalLineType.Include:
					return string.Format( IncludeFormat, LineValue );
					case AdditionalLineType.Define:
					return string.Format( DefineFormat, LineValue );
					case AdditionalLineType.Pragma:
					return string.Format( PragmaFormat, LineValue );
				}
				return LineValue;
			}
		}
	}

	public enum ReordableAction
	{
		None,
		Add,
		Remove
	}

	[Serializable]
	public sealed class TemplateAdditionalDirectivesHelper : TemplateModuleParent
	{
		private string NativeFoldoutStr = "Native";
		private const float ShaderKeywordButtonLayoutWidth = 15;

		[SerializeField]
		private List<AdditionalDirectiveContainer> m_additionalDirectives = new List<AdditionalDirectiveContainer>();
		
		[SerializeField]
		private List<AdditionalDirectiveContainer> m_shaderFunctionDirectives = new List<AdditionalDirectiveContainer>();

		[SerializeField]
		private List<string> m_nativeDirectives = new List<string>();

		[SerializeField]
		private bool m_nativeDirectivesFoldout = false;

		private ReordableAction m_actionType = ReordableAction.None;
		private int m_actionIndex = 0;
		private ReorderableList m_reordableList = null;
		private GUIStyle m_propertyAdjustment;


		public TemplateAdditionalDirectivesHelper( string moduleName ) : base( moduleName ) { }
		
		//public void AddShaderFunctionItem( AdditionalLineType type, string item )
		//{
		//	UpdateShaderFunctionDictionary();
		//	string id = type + item;
		//	if( !m_shaderFunctionDictionary.ContainsKey( id ) )
		//	{
		//		AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
		//		newItem.LineType = type;
		//		newItem.LineValue = item;
		//		newItem.hideFlags = HideFlags.HideAndDontSave;
		//		m_shaderFunctionDirectives.Add( newItem );
		//		m_shaderFunctionDictionary.Add( id, newItem );
		//	}
		//}

		public void AddShaderFunctionItems( List<AdditionalDirectiveContainer> functionList )
		{
			m_shaderFunctionDirectives.AddRange( functionList );				
		}

		public void RemoveShaderFunctionItems( List<AdditionalDirectiveContainer> functionList )
		{
			for( int i = 0; i < functionList.Count; i++ )
			{
				m_shaderFunctionDirectives.Remove( functionList[ i ] );
			}
		}

		//public void RemoveShaderFunctionItem( AdditionalLineType type, string item )
		//{
		//	m_shaderFunctionDirectives.RemoveAll( x => x.LineType == type && x.LineValue.Equals( item ) );
		//}

		public void AddItems( AdditionalLineType type, List<string> items )
		{
			int count = items.Count;
			for( int i = 0; i < count; i++ )
			{
				AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
				newItem.LineType = type;
				newItem.LineValue = items[ i ];
				newItem.hideFlags = HideFlags.HideAndDontSave;
				m_additionalDirectives.Add( newItem );
			}
		}

		public void FillNativeItems( List<string> nativeItems )
		{
			m_nativeDirectives.Clear();
			m_nativeDirectives.AddRange( nativeItems );
		}

		void DrawNativeItems()
		{
			EditorGUILayout.Separator();
			EditorGUI.indentLevel++;
			int count = m_nativeDirectives.Count;
			for( int i = 0; i < count; i++ )
			{
				EditorGUILayout.LabelField( m_nativeDirectives[ i ] );
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add keyword
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
				newItem.hideFlags = HideFlags.HideAndDontSave;
				m_additionalDirectives.Add( newItem );
				EditorGUI.FocusTextInControl( null );
				m_isDirty = true;
			}

			//Remove keyword
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_additionalDirectives.Count > 0 )
				{
					AdditionalDirectiveContainer itemToDelete = m_additionalDirectives[ m_additionalDirectives.Count - 1 ];
					m_additionalDirectives.RemoveAt( m_additionalDirectives.Count - 1 );
					ScriptableObject.DestroyImmediate( itemToDelete );
					EditorGUI.FocusTextInControl( null );
				}
				m_isDirty = true;
			}
		}

		public override void Draw( UndoParentNode owner, bool style = true )
		{
			if( m_reordableList == null )
			{
				m_reordableList = new ReorderableList( m_additionalDirectives, typeof( AdditionalDirectiveContainer ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,
					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						if( m_additionalDirectives[ index ] != null )
						{
							if( style )
							{
								rect.x -= 10;
							}
							else
							{
								rect.x -= 1;
							}

							float popUpWidth = style ? 0.35f : 0.29f;
							Rect popupPos = new Rect( rect.x, rect.y + 2, popUpWidth * rect.width, rect.height );
							Rect labelPos = new Rect( rect.x + popupPos.width, rect.y, 0.59f * rect.width, rect.height );
							Rect buttonPlusPos = new Rect( labelPos.x + labelPos.width, rect.y, ShaderKeywordButtonLayoutWidth, rect.height );
							Rect buttonMinusPos = new Rect( buttonPlusPos.x + buttonPlusPos.width, rect.y, ShaderKeywordButtonLayoutWidth, rect.height );

							m_additionalDirectives[ index ].LineType = (AdditionalLineType)owner.EditorGUIEnumPopup( popupPos, m_additionalDirectives[ index ].LineType );
							m_additionalDirectives[ index ].LineValue = owner.EditorGUITextField( labelPos, string.Empty, m_additionalDirectives[ index ].LineValue );
							if( GUI.Button( buttonPlusPos, string.Empty, UIUtils.PlusStyle ) )
							{
								m_actionType = ReordableAction.Add;
								m_actionIndex = index;
							}

							if( GUI.Button( buttonMinusPos, string.Empty, UIUtils.MinusStyle ) )
							{
								m_actionType = ReordableAction.Remove;
								m_actionIndex = index;
							}
						}
					}
				};
			}

			if( m_actionType != ReordableAction.None )
			{
				switch( m_actionType )
				{
					case ReordableAction.Add:
					AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					m_additionalDirectives.Insert( m_actionIndex, newItem );
					break;
					case ReordableAction.Remove:
					AdditionalDirectiveContainer itemToDelete = m_additionalDirectives[ m_actionIndex ];
					m_additionalDirectives.RemoveAt( m_actionIndex );
					ScriptableObject.DestroyImmediate( itemToDelete );
					break;
				}
				m_isDirty = true;
				m_actionType = ReordableAction.None;
				EditorGUI.FocusTextInControl( null );
			}
			bool foldoutValue = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedAdditionalDirectives;
			if( style )
			{
				NodeUtils.DrawPropertyGroup( ref foldoutValue, m_moduleName, DrawReordableList, DrawButtons );
			}
			else
			{
				NodeUtils.DrawNestedPropertyGroup( ref foldoutValue, m_moduleName, DrawReordableList, DrawButtons );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedAdditionalDirectives = foldoutValue;
		}

		void DrawReordableList()
		{
			if( m_reordableList != null )
			{
				if( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				//EditorGUILayout.BeginVertical( m_propertyAdjustment );
				EditorGUILayout.Space();
				if( m_nativeDirectives.Count > 0 )
				{
					NodeUtils.DrawNestedPropertyGroup( ref m_nativeDirectivesFoldout, NativeFoldoutStr, DrawNativeItems, 4 );
				}
				if( m_additionalDirectives.Count == 0 )
				{
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
				}
				else
				{
					m_reordableList.DoLayoutList();
				}
				EditorGUILayout.Space();
				//EditorGUILayout.EndVertical();
			}
		}

		public void AddAllToDataCollector( ref MasterNodeDataCollector dataCollector, TemplateIncludePragmaContainter nativesContainer )
		{
			AddToDataCollector( ref dataCollector, nativesContainer, false );
			AddToDataCollector( ref dataCollector, nativesContainer, true );
		}

		public void AddAllToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			AddToDataCollector( ref dataCollector, false );
			AddToDataCollector( ref dataCollector, true );
		}

		void AddToDataCollector( ref MasterNodeDataCollector dataCollector, TemplateIncludePragmaContainter nativesContainer, bool fromSF )
		{
			List<AdditionalDirectiveContainer> list = fromSF ? m_shaderFunctionDirectives : m_additionalDirectives;
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				switch( list[ i ].LineType )
				{
					case AdditionalLineType.Include:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) &&
						  !nativesContainer.HasInclude( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Define:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) &&
						  !nativesContainer.HasDefine( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Pragma:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) &&
						  !nativesContainer.HasPragma( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					default:
					case AdditionalLineType.Custom:
					dataCollector.AddToMisc( list[ i ].LineValue );
					break;
				}
			}
		}

		void AddToDataCollector( ref MasterNodeDataCollector dataCollector, bool fromSF )
		{
			List<AdditionalDirectiveContainer> list = fromSF ? m_shaderFunctionDirectives : m_additionalDirectives;
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				switch( list[ i ].LineType )
				{
					case AdditionalLineType.Include:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Define:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Pragma:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					default:
					case AdditionalLineType.Custom:
					dataCollector.AddToMisc( list[ i ].LineValue );
					break;
				}
			}
		}


		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			try
			{
				int count = Convert.ToInt32( nodeParams[ index++ ] );
				for( int i = 0; i < count; i++ )
				{
					AdditionalLineType lineType = (AdditionalLineType)Enum.Parse( typeof( AdditionalLineType ), nodeParams[ index++ ] );
					string lineValue = nodeParams[ index++ ];
					AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					newItem.LineType = lineType;
					newItem.LineValue = lineValue;
					m_additionalDirectives.Add( newItem );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDirectives.Count );
			for( int i = 0; i < m_additionalDirectives.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDirectives[ i ].LineType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDirectives[ i ].LineValue );
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_nativeDirectives.Clear();
			m_nativeDirectives = null;

			m_additionalDirectives.Clear();
			m_additionalDirectives = null;

			m_propertyAdjustment = null;
			m_reordableList = null;
		}

		public List<AdditionalDirectiveContainer> DirectivesList { get { return m_additionalDirectives; } }
		public bool IsValid { get { return m_validData; } set { m_validData = value; } }
	}
}
