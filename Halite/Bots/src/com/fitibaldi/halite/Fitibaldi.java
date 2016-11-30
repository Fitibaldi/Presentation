package com.fitibaldi.halite;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class Fitibaldi {
    private static final int COEFF_ATTACK = 5;
    private static final int COEFF_DEFENSE = 2;
    private static GameMap gameMap = null;

    public static void main(String[] args) throws java.io.IOException {
        InitPackage iPackage = Networking.getInit();
        int myID = iPackage.myID;
        gameMap = iPackage.map;

        Networking.sendInit("Fitibaldi");

        while (true) {
            ArrayList<Move> moves = new ArrayList<Move>();

            gameMap = Networking.getFrame();

            for (int y = 0; y < gameMap.height; y++) {
                for (int x = 0; x < gameMap.width; x++) {
                    Site site = gameMap.getSite(new Location(x, y));
                    if (site.owner == myID) {
                        Direction dir = getDirection(myID, site, new Location(x, y));
                        moves.add(new Move(new Location(x, y), dir));
                    }
                }
            }
            Networking.sendFrame(moves);
        }
    }

    private static Direction getDirection(int myID, Site site, Location location) {
        Direction direction = Direction.STILL;
        Integer maxGain = getPoints(myID, site, location, Direction.STILL);
        Integer currGain = getPoints(myID, site, location, Direction.EAST);
        if (maxGain < currGain) {
            direction = Direction.EAST;
            maxGain = currGain;
        }
        currGain = getPoints(myID, site, location, Direction.NORTH);
        if (maxGain < currGain) {
            direction = Direction.NORTH;
            maxGain = currGain;
        }
        currGain = getPoints(myID, site, location, Direction.SOUTH);
        if (maxGain < currGain) {
            direction = Direction.SOUTH;
            maxGain = currGain;
        }
        currGain = getPoints(myID, site, location, Direction.WEST);
        if (maxGain < currGain) {
            direction = Direction.WEST;
            maxGain = currGain;
        }
        return direction;
    }

    private static Integer getPoints(int myID, Site site, Location location, Direction direction) {
        Site opposite = gameMap.getSite(location, direction);
        Integer strength = 0;
        if (!Direction.STILL.equals(direction)) {
            strength = (opposite.strength * (myID == opposite.owner ? 1 : -1) + site.strength) * COEFF_ATTACK;
        }
        Integer production = opposite.production * COEFF_DEFENSE;
        Integer population = 0;
        if (!Direction.STILL.equals(direction)) {
            population = 1 * COEFF_DEFENSE;
        }
        return strength + production + population;
    }
}
