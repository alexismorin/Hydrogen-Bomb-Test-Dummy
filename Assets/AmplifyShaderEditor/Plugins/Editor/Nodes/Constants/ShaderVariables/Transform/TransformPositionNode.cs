// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Transform Position", "Object Transform", "Transforms a position value from one space to another" )]
	public sealed class TransformPositionNode : ParentNode
	{
		[SerializeField]
		private TransformSpace m_from = TransformSpace.Object;

		[SerializeField]
		private TransformSpace m_to = TransformSpace.World;

		private const string AseObjectToWorldPosVarName = "objToWorld";
		private const string AseObjectToWorldPosFormat = "mul( unity_ObjectToWorld, float4( {0}, 1 ) ).xyz";

		private const string AseObjectToViewPosVarName = "objToView";
		private const string AseObjectToViewPosFormat = "mul( UNITY_MATRIX_MV, float4( {0}, 1 ) ).xyz";

		private const string AseWorldToObjectPosVarName = "worldToObj";
		private const string AseWorldToObjectPosFormat = "mul( unity_WorldToObject, float4( {0}, 1 ) ).xyz";

		private const string AseWorldToViewPosVarName = "worldToView";
		private const string AseWorldToViewPosFormat = "mul( UNITY_MATRIX_V, float4( {0}, 1 ) ).xyz";

		private const string AseViewToObjectPosVarName = "viewToObj";
		private const string AseViewToObjectPosFormat = "mul( unity_WorldToObject, mul( UNITY_MATRIX_I_V , float4( {0}, 1 ) ) ).xyz";

		private const string AseViewToWorldPosVarName = "viewToWorld";
		private const string AseViewToWorldPosFormat = "mul( UNITY_MATRIX_I_V, float4( {0}, 1 ) ).xyz";

		private const string FromStr = "From";
		private const string ToStr = "To";
		private const string SubtitleFormat = "{0} to {1}";

		private readonly string[] m_spaceOptions =
		{
			"Object Space",
			"World Space",
			"View Space",
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, Constants.EmptyPortValue );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "74e4d859fbdb2c0468de3612145f4929";
			m_textLabelWidth = 100;
			UpdateSubtitle();
		}

		private void UpdateSubtitle()
		{
			SetAdditonalTitleText( string.Format( SubtitleFormat, m_from, m_to ) );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_from = (TransformSpace)EditorGUILayout.Popup( FromStr, (int)m_from, m_spaceOptions );
			m_to = (TransformSpace)EditorGUILayout.Popup( ToStr, (int)m_to, m_spaceOptions );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateSubtitle();
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			string result = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string varName = string.Empty;

			switch( m_from )
			{
				case TransformSpace.Object:
				switch( m_to )
				{
					default:
					case TransformSpace.Object:
					break;
					case TransformSpace.World:
					result = string.Format( AseObjectToWorldPosFormat, result );
					varName = AseObjectToWorldPosVarName + OutputId;
					break;
					case TransformSpace.View:
					result = string.Format( AseObjectToViewPosFormat, result );
					varName = AseObjectToViewPosVarName + OutputId;
					break;
				}
				break;
				case TransformSpace.World:
				switch( m_to )
				{
					case TransformSpace.Object:
					result = string.Format( AseWorldToObjectPosFormat, result );
					varName = AseWorldToObjectPosVarName + OutputId;
					break;
					default:
					case TransformSpace.World:
					break;
					case TransformSpace.View:
					result = string.Format( AseWorldToViewPosFormat, result );
					varName = AseWorldToViewPosVarName + OutputId;
					break;
				}
				break;
				case TransformSpace.View:
				switch( m_to )
				{
					case TransformSpace.Object:
					result = string.Format( AseViewToObjectPosFormat, result );
					varName = AseViewToObjectPosVarName + OutputId;
					break;
					case TransformSpace.World:
					result = string.Format( AseViewToWorldPosFormat, result );
					varName = AseViewToWorldPosVarName + OutputId;
					break;
					default:
					case TransformSpace.View:
					break;
				}
				break;
				default:
				break;
			}

			RegisterLocalVariable( 0, result, ref dataCollector, varName );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_from = (TransformSpace)Enum.Parse( typeof( TransformSpace ), GetCurrentParam( ref nodeParams ) );
			m_to = (TransformSpace)Enum.Parse( typeof( TransformSpace ), GetCurrentParam( ref nodeParams ) );
			UpdateSubtitle();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_from );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_to );
		}
	}
}
