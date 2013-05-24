using UnityEngine;
using System.Collections;

namespace OffDaRailz{
	public interface IUpgrade{
		void SetTarget(Transform GunPort);
		void EnableUpgrade();
		void DisableUpgrade();
		string GetName();
		bool IsAvailable();
	}
}
