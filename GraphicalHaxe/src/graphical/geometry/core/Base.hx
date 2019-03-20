package graphical.geometry.core;

@:expose("geometry.Base")
class Base {

  private var _boundingBox:AABB;

  @:isVar public var BoundingBox(get, set):AABB;

  function get_BoundingBox(){
    if(this._boundingBox == null){
      this._boundingBox = this.ComputeBoundingBox();
    }
    return this._boundingBox;
  }

  function set_BoundingBox(bbox){
    return this._boundingBox = bbox;
  }

  private function ComputeBoundingBox():AABB{
    return null;
  }

}