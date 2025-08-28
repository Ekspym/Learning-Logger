const routes = [
  {
    path: '/',
    component: () => import('src/layouts/LogLayout.vue'),
    children: [
      {
        path: '',
        component: () => import('src/pages/LogPage.vue'),
        children: [
          // detail by ID
          {
            path: 'detail/:id',
            name: 'log-detail',
            component: () => import('src/pages/LogDetail.vue'),
            props: true,
          },
        ],
      },
    ],
  },
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
