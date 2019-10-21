///////////////////////////////////////////////////////////////////////////
// CsGraph.cs - Generic node and graph classes - Demo for Project #4     //
//                                                                       //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Spring 2010     //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsGraph
{
  /////////////////////////////////////////////////////////////////////////
  // CsNode<V,E> class

  struct pair<V, E> // holds child node and instance of edge type E
  { 
    public CsNode<V, E> node { get; set; }
    public E edgeValue { get; set; }
  };

  class CsNode<V,E>
  {
    //----< construct a named node >---------------------------------------

    public CsNode(string nodeName)
    {
      name = nodeName;
      children = new List<pair<V,E>>();
      visited = false;
    }
    //----< has been visited property >------------------------------------

    public bool visited
    {
      get;
      set;
    }
    //----< node name property >-------------------------------------------

    public string name
    {
      get;
      set;
    }
    //----< vertex value property >----------------------------------------

    public V nodeValue
    {
      get;
      set;
    }
    //----< child vertices property >--------------------------------------

    public List<pair<V, E>> children
    {
      get;
      set;
    }
    //----< add child vertex and its associated edge value to vertex >-----

    public void addChild(CsNode<V,E> childNode, E edgeVal)
    {
      children.Add(new pair<V, E> { node = childNode, edgeValue = edgeVal });
    }
    //----< find the next unvisited child >--------------------------------

    public pair<V, E> getNextUnmarkedChild()
    {
      foreach (pair<V, E> child in children)
      {
        if (!child.node.visited)
        {
          child.node.visited = true;
          return child;
        }
      }
      return new pair<V,E> { node = null, edgeValue = default(E) };
    }
    public void unmark() 
    { 
      visited = false; 
    }
    public override string ToString()
    {
      string temp = name;
      temp += ", " + children.Count.ToString();
      if (children.Count == 1)
        temp += " child node";
      else
        temp += " child nodes";
      return temp;
    }
  }
  /////////////////////////////////////////////////////////////////////////
  // Operation<V,E> class

  class Operation<V, E>
  {
    //----< graph.walk() calls this on every node >------------------------

    virtual public bool doNodeOp(CsNode<V,E> node)
    {
      Console.Write("\n  {0}", node.ToString());
      return true;
    }
    //----< graph calls this on every child visitation >-------------------

    virtual public bool doEdgeOp(E edgeVal)
    {
      Console.Write("\n  {0}", edgeVal.ToString());
      return true;
    }
  }
  /////////////////////////////////////////////////////////////////////////
  // CsGraph<V,E> class

  class CsGraph<V,E>
  {
    //----< construct a named graph >--------------------------------------

    public CsGraph(string graphName)
    {
      name = graphName;
      adjList = new List<CsNode<V, E>>();
      gop = new Operation<V, E>();
      startNode = 0;
    }
    //----< register an Operation with the graph >-------------------------

    public Operation<V, E> setOperation(Operation<V, E> newOp)
    {
      Operation<V, E> temp = gop;
      gop = newOp;
      return temp;
    }
    //----< walk starting node property >----------------------------------

    public int startNode
    {
      get;
      set;
    }
    //----< graph name property >------------------------------------------

    public string name
    {
      get;
      set;
    }
    //----< adjacency list property >--------------------------------------

    private List<CsNode<V, E>> adjList
    {
      get;
      set;
    }
    //----< add vertex to graph adjacency list >---------------------------

    public void addNode(CsNode<V,E> node)
    {
      adjList.Add(node);
    }
    //----< clear visitation marks to prepare for next walk >--------------

    public void clearMarks()
    {
      foreach (CsNode<V, E> node in adjList)
        node.unmark();
    }
    //----< depth first search from startNode >----------------------------

    public void walk()
    {
      if(adjList.Count == 0)
        return;
      this.walk(adjList[startNode]);
      foreach (CsNode<V, E> node in adjList)
        if (!node.visited)
          walk(node);
      foreach (CsNode<V, E> node in adjList)
        node.unmark();
      return;
    }
    //----< depth first search from specific node >------------------------

    public bool walk(CsNode<V,E> node)
    {
      // process this node

      node.visited = true;
      if (gop == null)
        throw new Exception("no graph operation defined");
      gop.doNodeOp(node);

      // visit children
      do
      {
        pair<V,E> childPair = node.getNextUnmarkedChild();
        if (childPair.node == null)
          return true;
        gop.doEdgeOp(childPair.edgeValue);
        walk(childPair.node);
      } while (true);
      // never reach this point
    }
    private Operation<V, E> gop = null;
  }
  /////////////////////////////////////////////////////////////////////////
  // Test class

  class demoOperation : Operation<string, string>
  {
    override public bool doNodeOp(CsNode<string, string> node)
    {
      Console.Write("\n  {0},", node.name);
      return true;
    }
  }
  class Test
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Testing CsGraph class");
      Console.Write("\n =======================");

      CsNode<string, string> node1 = new CsNode<string, string>("node1");
      CsNode<string, string> node2 = new CsNode<string, string>("node2");
      CsNode<string, string> node3 = new CsNode<string, string>("node3");
      CsNode<string, string> node4 = new CsNode<string, string>("node4");
      CsNode<string, string> node5 = new CsNode<string, string>("node5");
      
      node1.addChild(node2,"child of node1");
      node1.addChild(node3,"child of node1");
      node2.addChild(node4,"child of node2");
      node5.addChild(node1,"child of node5");
      node3.addChild(node1,"child of node3");

      CsGraph<string,string> graph = new CsGraph<string,string>("Fred");
      graph.addNode(node1);
      graph.addNode(node2);
      graph.addNode(node3);
      graph.addNode(node4);
      graph.addNode(node5);

      Console.Write("\n\n  starting walk at adjList[{0}]", graph.startNode);
      graph.walk();

      graph.startNode = 2;
      Console.Write("\n\n  starting walk at adjList[{0}]", graph.startNode);
      graph.setOperation(new demoOperation());
      graph.walk();

      Console.Write("\n\n");
    }
  }
}
