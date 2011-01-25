class Graph:
  nodes = []
  edges = []

  def neighbors(__this__,v):
    n = {}
    for e in __this__.edges:
      if e[0] == v:
        n[e[1]] = ""
      if e[1] == v:
        n[e[0]] = ""
    return n

  def eccentricity(__this__):
    nodes=[]
    nodes=__this__.nodes
    order=len(__this__.nodes)
    e={}
    for v in nodes:
        length=__this__.shortest_path(v)
        L = len(length)
        e[v]=max(length.values())
    if len(e)==1: return list(e.values())[0] 
    else:
        return e

  def centers(__this__):
    e=__this__.eccentricity()
    radius=min(e.values())
    p=[v for v in e if e[v]==radius]
    return p

  def shortest_path(__this__,source):
    seen={}                
    level=0               
    nextlevel={source:1} 
    while nextlevel:
      thislevel=nextlevel
      nextlevel={}       
      for v in thislevel:
        if v not in seen: 
          seen[v]=level
          nextlevel.update(__this__.neighbors(v))
      level=level+1
    return seen

  def unique(__this__, s):
    n = len(s)
    if n == 0:
        return []
    u = {}
    try:
        for x in s:
            u[x] = 1
    except TypeError:
        del u
    else:
        return u.keys()
    try:
        t = list(s)
        t.sort()
    except TypeError:
        del t
    else:
        assert n > 0
        last = t[0]
        lasti = i = 1
        while i < n:
            if t[i] != last:
                t[lasti] = last = t[i]
                lasti += 1
            i += 1
        return t[:lasti]

    u = []
    for x in s:
        if x not in u:
            u.append(x)
    return u

 
def center(str):
  arg = []
  argv = str.split(' ')
  for args in argv:
    arg.append(args)
  g=Graph()
  for args in arg:
    s = args.split(',')
    g.edges.append([s[0], s[1]])
    g.nodes.append(s[0])
    g.nodes.append(s[1])
    g.edges = g.unique(g.edges)
    g.nodes = g.unique(g.nodes)
  return g.centers()
ress = center(ret)
res = ""
for x in ress:
  res = res + x + " "