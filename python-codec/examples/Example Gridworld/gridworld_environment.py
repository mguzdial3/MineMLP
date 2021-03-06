# 
# Copyright (C) 2009, Brian Tanner
# 
#http://rl-glue-ext.googlecode.com/
#
# Licensed under the Apache License, Version 2.0 (the "License")
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
#  $Revision: 999 $
#  $Date: 2009-02-09 09:39:12 -0700 (Mon, 09 Feb 2009) $
#  $Author: brian@tannerpages.com $
#  $HeadURL: http://rl-library.googlecode.com/svn/trunk/projects/packages/examples/mines-sarsa-python/sample_mines_environment.py $

import random
import sys
from rlglue.environment.Environment import Environment
from rlglue.environment import EnvironmentLoader as EnvironmentLoader
from rlglue.types import Observation
from rlglue.types import Action
from rlglue.types import Reward_observation_terminal

# This is a very simple discrete-state, episodic grid world that has 
# exploding mines in it.  If the agent steps on a mine, the episode
# ends with a large negative reward.
# 
# The reward per step is -1, with +10 for exiting the game successfully
# and -100 for stepping on a mine.


# TO USE THIS Environment [order doesn't matter]
# NOTE: I'm assuming the Python codec is installed an is in your Python path
#   -  Start the rl_glue executable socket server on your computer
#   -  Run the SampleSarsaAgent and SampleExperiment from this or a
#   different codec (Matlab, Python, Java, C, Lisp should all be fine)
#   -  Start this environment like:
#   $> python sample_mines_environment.py

class mines_environment(Environment):
    WORLD_FREE = 0
    WORLD_OBSTACLE = 2
    WORLD_GOAL = 3
    WORLD_RADIOACTIVE = 5
    randGenerator=random.Random()
    fixedStartState=False
    startRow=4
    startCol=4
    lastActionValue = -1
    numGoals = 5;
    currentState=10
    def env_init(self):
        dimX = 31
        dimY = 32

        goalX = 9
        goalY = 9
        self.map = [[2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2], 
                    [2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2], 
                    [2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 2, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 2, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 2, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 2, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 2, 2, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 2, 5, 5, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0, 2],
                    [2, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 3, 0, 0, 5, 5, 5, 5, 5, 0, 2],
                    [2, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 5, 5, 2, 5, 5, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0, 2],
                    [2, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 5, 5, 2, 5, 5, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0, 2],
                    [2, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 5, 5, 2, 5, 5, 0, 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0, 2],
                    [2, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2],
                    [2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2]]
        #self.map = []
        #for i in range(dimY):
        #    temp = []
        #    for j in range(dimX):
        #        if i != 0 and i != dimY-1 and j != 0 and j != dimX-1:
        #            temp.append(0)
        #        else:
        #            temp.append(1)

        #    self.map.append(temp)

        #self.map[goalY][goalX] = 3
        
        print self.map
        #Options list contains options. [StartRow, StartCol, EndRow, EndCol, numSteps]
        self.optionsList = [[1, 4, 2, 8, 4], [1, 4, 4, 6, 4]]
        #self.optionsList = []
        self.optionsArray = []
        for i in range(len(self.map)):
            temp = []
            for j in range(len(self.map[i])):
                temp1 = []
                for k in range(len(self.optionsList)):
                    if i == self.optionsList[k][0] and j == self.optionsList[k][1]:
                        temp1.append(k)
                temp.append(temp1)
            self.optionsArray.append(temp)
        for i in self.optionsArray:
            print i    

        #The Python task spec parser is not yet able to build task specs programmatically
        #So we have some special observations. Lenght 4 character array. Index 0 is up, 1 is down, 2 is right, 3 is left! We should expand this out so that size is based on the total number of possible actions to take (you need one for every option)
        #Currently have 5 actions, 3 directions + 2 options
        return "VERSION RL-Glue-3.0 PROBLEMTYPE episodic DISCOUNTFACTOR 1 OBSERVATIONS INTS (0 121) CHARCOUNT 5 ACTIONS INTS (0 4) REWARDS (-100.0 10.0) EXTRA gridWorldEnvironment by Brent Harrison."
    
    def env_start(self):
        if self.fixedStartState:
            stateValid=self.setAgentState(self.startRow,self.startCol)
            if not stateValid:
                print "The fixed start state was NOT valid: "+str(int(self.startRow))+","+str(int(self.startRow))
                self.setRandomState()
        else:
            self.setRandomState()

        returnObs=Observation()
        returnObs.intArray=[self.calculateFlatState()]
        #Up, Right, Down, Option1, Option2
        returnObs.charArray = ["T", "T", "T", "T"]
        if len(self.optionsArray[self.startRow][self.startCol]) != 0:
            for i in range(len(self.optionsArray[self.startRow][self.startCol])):
                returnObs.charArray[3+self.optionsArray[self.startRow][self.startCol][i]] = "T"
       # print returnObs.charArray
        #Now add characters based on options present
        
        

        return returnObs
        
    def env_step(self,thisAction):
        # Make sure the action is valid
        assert len(thisAction.intArray)==1,"Expected 1 integer action."
        assert thisAction.intArray[0]>=0, "Expected action to be in [0,4]"
        assert thisAction.intArray[0]<4, "Expected action to be in [0,4]"
        
        self.updatePosition(thisAction.intArray[0])

        lastActionValue = thisAction.intArray[0]
        theObs=Observation()
        theObs.intArray=[self.calculateFlatState()]
        theObs.charArray = ["T", "T", "T", "T"]
        if len(self.optionsArray[self.agentRow][self.agentCol]) != 0:
            for i in range(len(self.optionsArray[self.agentRow][self.agentCol])):
                theObs.charArray[2+self.optionsArray[self.agentRow][self.agentCol][i]] = "T"
        
        returnRO=Reward_observation_terminal()
        returnRO.r=self.calculateReward(lastActionValue)
        returnRO.o=theObs
        returnRO.terminal=self.checkCurrentTerminal()

        return returnRO

    def env_cleanup(self):
        pass

    def env_message(self,inMessage):
        #   Message Description
        # 'set-random-start-state'
        #Action: Set flag to do random starting states (the default)
        if inMessage.startswith("set-random-start-state"):
            self.fixedStartState=False;
            return "Message understood.  Using random start state.";

        #   Message Description
        # 'set-start-state X Y'
        # Action: Set flag to do fixed starting states (row=X, col=Y)
        if inMessage.startswith("set-start-state"):
            splitString=inMessage.split(" ");
            self.startRow=int(splitString[1]);
            self.startCol=int(splitString[2]);
            self.fixedStartState=True;
            return "Message understood.  Using fixed start state.";

        #   Message Description
        #   'print-state'
        #   Action: Print the map and the current agent location
        if inMessage.startswith("print-state"):
            self.printState();
            return "Message understood.  Printed the state.";

        return "SamplesMinesEnvironment(Python) does not respond to that message.";

    def setAgentState(self,row, col):
        self.agentRow=row
        self.agentCol=col

        return self.checkValid(row,col) and not self.checkTerminal(row,col)

    def setRandomState(self):
        numRows=len(self.map)
        numCols=len(self.map[0])
        startRow=self.randGenerator.randint(0,numRows-1)
        startCol=self.randGenerator.randint(0,numCols-1)

        while not self.setAgentState(startRow,startCol):
            startRow=self.randGenerator.randint(0,numRows-1)
            startCol=self.randGenerator.randint(0,numCols-1)

    def checkValid(self,row, col):
        valid=False
        numRows=len(self.map)
        numCols=len(self.map[0])

        if(row < numRows and row >= 0 and col < numCols and col >= 0):
            if self.map[row][col] != self.WORLD_OBSTACLE:
                valid=True
        return valid

    def checkTerminal(self,row,col):
        if (self.map[row][col] == self.WORLD_GOAL and self.numGoals==1):
            return True

        if(self.map[row][col]==self.WORLD_GOAL):
            self.map[row][col] = self.WORLD_FREE
            self.numGoals-=1;
        
        return False

    def checkCurrentTerminal(self):
        return self.checkTerminal(self.agentRow,self.agentCol)

    def calculateFlatState(self):
        numRows=len(self.map)
        numCols=len(self.map[0])
        
        return self.agentRow * numCols + self.agentCol
        #I don't think this works..except that they're using row and column really wierdly
        #return self.agentCol * numRows + self.agentRow



    def updatePosition(self, theAction):
        # When the move would result in hitting an obstacles, the agent simply doesn't move 
        newRow = self.agentRow;
        newCol = self.agentCol;

        if(self.map[self.agentRow][self.agentCol] != self.WORLD_RADIOACTIVE):
            randomChance = random.random();
            if (randomChance<0.25):#move down
                newRow = self.agentRow + 1;
            elif (randomChance<0.5): #move up
                newRow = self.agentRow + -1;
            elif (randomChance<0.75):#move right
                newCol = self.agentCol + 1;
            elif (randomChance<1):#move left
                newCol = self.agentCol + -1;

        else:
            if (theAction == 0):#move down
                newRow = self.agentRow + 1;
            elif (theAction == 1): #move up
                newRow = self.agentRow + -1;
            elif (theAction == 2):#move right
                newCol = self.agentCol + 1;
            elif (theAction == 3):#move left
                newCol = self.agentCol + -1;
                


        #Check if new position is out of bounds or inside an obstacle 
        if(self.checkValid(newRow,newCol)):
            self.agentRow = newRow;
            self.agentCol = newCol;

    def calculateReward(self, theAction):
        if(self.map[self.agentRow][self.agentCol] == self.WORLD_GOAL and theAction > 3):
            return 10.0 + (-1.0*(self.optionsList[theAction-3][4]-1))
        if(self.map[self.agentRow][self.agentCol] == self.WORLD_GOAL):
            return 10.0
        return -1.0;
        
    def printState(self):
        numRows=len(self.map)
        numCols=len(self.map[0])
        print "Agent is at: "+str(self.agentRow)+","+str(self.agentCol)
        print "Columns:0-10                10-17"
        print "Col    ",
        for col in range(0,numCols):
            print col%10,
            
        for row in range(0,numRows):
            print
            print "Row: "+str(row)+" ",
            for col in range(0,numCols):
                if self.agentRow==row and self.agentCol==col:
                    print "A",
                else:
                    if self.map[row][col] == self.WORLD_GOAL:
                        print "G",
                    if self.map[row][col] == self.WORLD_OBSTACLE:
                        print "*",
                    if self.map[row][col] == self.WORLD_FREE:
                        print " ",
                    if self.map[row][col] == self.WORLD_RADIOACTIVE:
                        print "R",
        print
        

if __name__=="__main__":
    EnvironmentLoader.loadEnvironment(mines_environment())
