﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIBike : Bike
{
    public float turnTime = 2f;

    public float aiCheckTimeout = .25f;

    public float secsSinceLastAiCheck = 0;

    public float maxX = Ground.maxX - 10*Ground.gridSize; // assumes min === -max
    public float maxZ = Ground.maxZ - 10*Ground.gridSize;

    public override void DecideToTurn()
    {
        Vector3 pos = transform.position;

        Ground g = GameMain.GetInstance().ground;

        if (_curTurn == TurnDir.kStraight) { // not currently turning
            secsSinceLastAiCheck += Time.deltaTime;   
            if (secsSinceLastAiCheck > aiCheckTimeout) {         
                secsSinceLastAiCheck = 0;

                // If not gonna turn maybe go towards the closest bike?
                if (_pendingTurn == TurnDir.kUnset) {
                    Vector3 closestBikePos = ClosestBike(this.gameObject).transform.position;
                    if ( Vector3.Distance(pos, closestBikePos) > Ground.gridSize * 6) // only if it's not really close
                        _pendingTurn = TurnTowardsPos( closestBikePos, pos, heading ); 
                    else
                    {
                        bool doTurn = ( Random.value * turnTime <  GameTime.DeltaTime() );
                        if (doTurn)    
                            _pendingTurn = (Random.value < .5f) ? TurnDir.kLeft : TurnDir.kRight;                        
                    }               
                }

                // Do some looking ahaed
                Vector3 nextPos = UpcomingGridPoint(pos, heading);



                MoveNode moveTree = BuildMoveTree(nextPos, heading, 4);
                List<dirAndScore> dirScores = TurnScores(moveTree);
                dirAndScore best =  SelectBestTurn(dirScores); // dirScores.OrderBy( ds => ds.score).Last(); // Add some randomness for ties?
                if (  _pendingTurn == TurnDir.kUnset || dirScores[(int)_pendingTurn].score < best.score) 
                {
                    //Debug.Log(string.Format("New Turn: {0}", best.turnDir));                    
                    _pendingTurn =  best.turnDir;
                }
            }   
        }

    }

    protected dirAndScore SelectBestTurn(List<dirAndScore> dirScores) {
        int bestScore = dirScores.OrderBy( ds => ds.score).Last().score;
        // If you only take the best score you will almost always just go forwards.
        List<dirAndScore> turns = dirScores.Where( ds => ds.score >= bestScore * .5).ToList();
        int sel = (int)(Random.value * (float)turns.Count);
        //Debug.Log(string.Format("Possible: {0}, Sel Idx: {1}", turns.Count, sel));
        return turns[sel];
    }

}
