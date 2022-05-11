using UnityEngine;
using System.Collections.Generic;
using OneKnight;

namespace Itch {
    public class FollowerManager : MonoBehaviour {
        //serializing these for debugging
        [SerializeField]
        private int costLimit = 0;
        [SerializeField]
        private int currentCost = 0;
        public bool mostRecentPriority;

        [SerializeField]
        private List<Tamed> followers;

        public bool ToggleFollower(Tamed follower) {
            if(followers.Remove(follower)) {
                RemoveFollower(follower);
                Notifications.CreateNotification(transform.position, costLimit - currentCost + " unused Handling points");
                return true;
            }
            return TryAddFollower(follower);
        }

        private void RemoveFollower(Tamed follower) {
            currentCost -= follower.handlingCost;
            follower.EndFollow();
        }

        public bool TryAddFollower(Tamed follower) {
            if(currentCost + follower.handlingCost <= costLimit) {
                if(!followers.Contains(follower)) {
                    AddFollower(follower);
                    return true;
                } else {
                    Debug.Log("Attempted to add a follower that is already following. This should not happen.");
                }
            } else {
                int needed = (currentCost + follower.handlingCost) - costLimit;
                Notifications.CreateError(transform.position, "Need " + needed + " more unused Handling points");
            }
            return false;
        }

        private void AddFollower(Tamed follower) {
            followers.Add(follower);
            follower.StartFollow(this);
            currentCost += follower.handlingCost;
            Notifications.CreateNotification(transform.position, "-" + follower.handlingCost + " Handling points. " + (costLimit - currentCost) + " remaining");
        }

        public void SetHandlingCost(int newCost) {
            costLimit = newCost;
            if(newCost >= currentCost)
                return;
            int costSoFar = 0;
            int index;
            if(mostRecentPriority) {
                index = followers.Count-1;
                while(index >= 0) {
                    if(costSoFar + followers[index].handlingCost <= costLimit) {
                        costSoFar += followers[index].handlingCost;
                    } else {
                        RemoveFollower(followers[index]);
                        followers.RemoveAt(index);
                    }
                    index--;
                }
            } else {
                index = 0;
                while(index < followers.Count) {
                    if(costSoFar + followers[index].handlingCost <= costLimit) {
                        costSoFar += followers[index].handlingCost;
                        index++;
                    } else {
                        RemoveFollower(followers[index]);
                        followers.RemoveAt(index);
                    }
                }
            }
            currentCost = costSoFar;
        }

        public void FormUp() {
            int index = 0;
            foreach(Vector2 pos in Utils.UniformRadialPositions(followers.Count, 1)) {
                followers[index].transform.position = pos;
                index++;
            }
        }
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}