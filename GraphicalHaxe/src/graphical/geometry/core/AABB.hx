package graphical.geometry.core;

class AABB {
    private var min:Array<Float>;
    private var max:Array<Float>;

    public function new(min:Array<Float>, max:Array<Float>){
        this.min = min;
        this.max = max;
    }

    
    public function Intersects(other:AABB){
        return  (min[0] <= other.max[0]) && (max[0] >= other.min[0]) &&
                (min[1] <= other.max[1]) && (max[1] >= other.min[1]) &&
                (min[2] <= other.max[2]) && (max[2] >= other.min[2]);
    }
}