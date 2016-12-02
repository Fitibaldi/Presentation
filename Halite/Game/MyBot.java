import java.util.ArrayList;

public class MyBot {
    private static final int COEFF_ATTACK = 70;
    private static final int COEFF_DEFENSE = 20;
    private static final int COEFF_POPULATION = 10;
    private static final int COEFF_NEGATIVE = -1;
    private static final int MINIMUM_STRENGTH = 5;
    private static final int MAXIMUM_STRENGTH = 200;
    private static final Direction[] bigDirection = {
        Direction.WEST, Direction.SOUTH, Direction.EAST, Direction.NORTH };
    private static final Direction[] smallDirection = {
        Direction.NORTH, Direction.SOUTH, Direction.EAST, Direction.WEST };

    public static void main(String[] args) throws java.io.IOException {
        InitPackage iPackage = Networking.getInit();
        int myID = iPackage.myID;
        GameMap gameMap = iPackage.map;

        Networking.sendInit("Fitibaldi");

        while (true) {
            ArrayList<Move> moves = new ArrayList<Move>();

            gameMap = Networking.getFrame();

            for (int y = 0; y < gameMap.height; y++) {
                for (int x = 0; x < gameMap.width; x++) {
                    Site site = gameMap.getSite(new Location(x, y));
                    if (site.owner == myID) {
                        Direction dir;
                        if (site.strength < MINIMUM_STRENGTH) {
                            dir = Direction.STILL;
                        } else {
                            dir = getDirection(gameMap, myID, site, new Location(x, y));
                        }
                        moves.add(new Move(new Location(x, y), dir));
                    }
                }
            }
            Networking.sendFrame(moves);
        }
    }

    private static Direction getDirection(GameMap gameMap, int myID, Site site, Location location) {
        //check the situation
        ArrayList<Neighbor> neighbors = new ArrayList<Neighbor>();
        neighbors.add(new Neighbor(Direction.STILL, false, site.strength, site.production));
        Direction[] directionArray = bigDirection;
        for (Direction currDirection : directionArray) {
            neighbors.add(getNeighbor(gameMap, myID, location, currDirection));
        }

        Decision siteDecision = getDecision(neighbors);

        return getDirection(siteDecision, neighbors);
    }

    private static Neighbor getNeighbor(GameMap gameMap, int myID, Location location, Direction direction) {
        Site opposite = gameMap.getSite(location, direction);
        return new Neighbor(direction, (opposite.owner == myID ? false : true), opposite.strength, opposite.production);
    }

    private static Decision getDecision(ArrayList<Neighbor> neighbors) {
        int enemies = 0;
        for (Neighbor n : neighbors) {
            if (n.enemy) {
                enemies++;
            }
        }

        if (enemies == 0) {
            return Decision.INTERNAL;
        } else if (enemies == 1) {
            return Decision.SIDE;
        } else if (enemies == 2) {
            return Decision.CORNER;
        } else {
            return Decision.ATTACKED;
        }
    }

    private static Direction getDirection(Decision siteDecision, ArrayList<Neighbor> neighbors) {
        if (Decision.INTERNAL.equals(siteDecision)) {
            return getInternalDirection(neighbors);
        } else if (Decision.SIDE.equals(siteDecision)) {
            return getSideDirection(neighbors);
        } else if (Decision.CORNER.equals(siteDecision)) {
            return getCornerDirection(neighbors);
        } else if (Decision.ATTACKED.equals(siteDecision)) {
            return getAttackedDirection(neighbors);
        }
        return Direction.STILL;
    }

    private static Direction getInternalDirection(ArrayList<Neighbor> neighbors) {
        int maxProduction = -1;
        Direction dir = Direction.STILL;
        for (Neighbor n : neighbors) {
            if (maxProduction < n.strength + n.production) {
                if (!Direction.STILL.equals(n.direction) && n.strength < MAXIMUM_STRENGTH) {
                    maxProduction = n.strength + n.production;
                    dir = n.direction;
                }
            }
        }
        return dir;
    }

    private static Direction getSideDirection(ArrayList<Neighbor> neighbors) {
        return Direction.STILL;
    }

    private static Direction getCornerDirection(ArrayList<Neighbor> neighbors) {
        return Direction.STILL;
    }

    private static Direction getAttackedDirection(ArrayList<Neighbor> neighbors) {
        Direction attackDirection = Direction.STILL;
        Integer strength = 0;
        int maxAttack = -1000000;
        int maxDefense = 0;
        Neighbor site =  neighbors.get(0);
        for (Neighbor opposite : neighbors) {
            if (opposite.enemy) {
                strength = (opposite.strength * COEFF_NEGATIVE + site.strength) * COEFF_ATTACK;
                if (strength > 0) {
                    strength += COEFF_POPULATION;
                    strength += (strength + opposite.production) * COEFF_DEFENSE;
                }
                if (maxAttack < strength) {
                    maxAttack = strength;
                    attackDirection = opposite.direction;
                }
            }
        }
        return attackDirection;
    }

    private static Integer getPoints(GameMap gameMap, int myID, Site site, Location location, Direction direction) {
        Site opposite = gameMap.getSite(location, direction);
        Integer strength = 0;
        Integer production = 0;
        Integer population = 0;
        Integer overStrength = 0;
        if (!Direction.STILL.equals(direction)) {
            if (myID == opposite.owner) {
                strength = (opposite.strength + site.strength) * COEFF_DEFENSE;
                if (opposite.strength == 0 || opposite.strength >= 200) {
                    strength *= COEFF_NEGATIVE;
                }
            } else {
                strength = (opposite.strength * COEFF_NEGATIVE + site.strength) * COEFF_ATTACK;
                if (strength > 0) {
                    population = COEFF_POPULATION;
                    production = (strength + opposite.production) * COEFF_DEFENSE;
                }
            }
        } else {
            production = site.production * COEFF_DEFENSE;
        }
        if (site.strength > 240) {
            if (myID != opposite.owner) {
                overStrength = opposite.strength * COEFF_ATTACK;
            } else {
                overStrength = opposite.strength * COEFF_NEGATIVE;
            }
        } else {
            if (strength > 255) {
                overStrength = opposite.strength * COEFF_ATTACK * COEFF_NEGATIVE;
            }
        }
        return strength + production + population + overStrength;
    }
}
