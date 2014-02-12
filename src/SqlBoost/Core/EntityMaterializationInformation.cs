using System;
using System.Linq;
using System.Reflection;

namespace SqlBoost.Core
{
	public class EntityMaterializationInformation
	{
		public bool IsConstructorBased
		{
			get { return ConstructorInfo != null; }
		}
		public bool IsSingleValue
		{
			get { return SingleValueType != null; }
		}
		public Type SingleValueType { get; private set; }
		public ConstructorInfo ConstructorInfo { get; private set; }
		public int[] FieldsIndexes { get; private set; }
		public EntityMaterializationInformation(Type singleValueType)
			: this(singleValueType, null, null)
		{
		}
		public EntityMaterializationInformation(ConstructorInfo ctorInfo)
			: this(null, ctorInfo, null)
		{
		}
		public EntityMaterializationInformation(int[] fieldsIndexes)
			: this(null, null, fieldsIndexes)
		{
		}
		private EntityMaterializationInformation(
			Type singleValueType, ConstructorInfo ctorInfo, int[] fieldsIndexes)
		{
			ConstructorInfo = ctorInfo;
			FieldsIndexes = fieldsIndexes;
			SingleValueType = singleValueType;
		}
		public override int GetHashCode()
		{
			unchecked
			{
				if (IsSingleValue)
					return SingleValueType.GetHashCode();
				if (IsConstructorBased)
					return ConstructorInfo.GetHashCode();

				var result = 31;
				for (var i = 0; i < FieldsIndexes.Length; i++)
					result = (result * 31) ^ FieldsIndexes[i];
				return result;
			}
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			var inst = obj as EntityMaterializationInformation;
			if (inst == null)
				return false;
			if (IsSingleValue != inst.IsSingleValue)
				return false;
			if (IsSingleValue && SingleValueType == inst.SingleValueType)
				return true;
			if (!Equals(ConstructorInfo, inst.ConstructorInfo))
				return false;
			if (IsConstructorBased != inst.IsConstructorBased)
				return false;
			if (!IsConstructorBased && !FieldsIndexes.SequenceEqual(inst.FieldsIndexes))
				return false;
			return true;
		}
	}
}
